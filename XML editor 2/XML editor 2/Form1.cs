using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace XML_editor_2
{
    public partial class Form1 : Form
    {
        private XmlDocument xmlDoc;
        private string filePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "XML files (*.xml)|*.xml";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;

                        // Load the XML document
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(filePath);

                        // Clear the treeView
                        treeView.Nodes.Clear();

                        // Populate the treeView
                        PopulateTreeView(xmlDoc.DocumentElement, treeView.Nodes);

                        // Set the text box content
                        xmlTextBox.Text = xmlDoc.OuterXml;

                        // Update the status label
                        statusLabel.Text = "XML file loaded successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error loading XML file.";
            }
        }

        private void PopulateTreeView(XmlNode xmlNode, TreeNodeCollection treeNodes)
        {
            if (xmlNode.NodeType == XmlNodeType.Element)
            {
                string nodeName = xmlNode.Name;
                string nodeText = nodeName;

                if (nodeName == "group" && xmlNode.Attributes != null && xmlNode.Attributes["name"] != null)
                {
                    nodeText = xmlNode.Attributes["name"].Value;
                }
                else if (nodeName == "subgroup" && xmlNode.Attributes != null && xmlNode.Attributes["name"] != null)
                {
                    nodeText = xmlNode.Attributes["name"].Value;
                }
                else if (nodeName == "item" && xmlNode.Attributes != null && xmlNode.Attributes["name"] != null)
                {
                    string itemName = xmlNode.Attributes["name"].Value;
                    string itemAttributes = string.Empty;

                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        if (attribute.Name != "name")
                        {
                            itemAttributes += $"{attribute.Name}: {attribute.Value} - ";
                        }
                    }

                    itemAttributes = itemAttributes.TrimEnd(' ', '-'); // Remove trailing space and hyphen

                    nodeText = $"{itemName} - {itemAttributes}";
                }

                TreeNode elementNode = new TreeNode(nodeText);

                if (xmlNode.HasChildNodes)
                {
                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                    {
                        PopulateTreeView(childNode, elementNode.Nodes);
                    }
                }

                treeNodes.Add(elementNode);
            }
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Disable renaming for the root node
            if (e.Node.Parent == null)
            {
                e.CancelEdit = true;
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // If the label was edited and not cancelled
            if (e.Label != null)
            {
                // Perform the necessary logic to update the node's name or process the new label
                // For example, you can update the XML document with the new name
                string newName = e.Label;
                // Update the XML document with the new name
                // ...

                // Cancel the edit operation if needed
                // For example, if the new name is invalid or already exists
                bool cancelEdit = false;
                if (cancelEdit)
                {
                    e.CancelEdit = true;
                }
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Check if the double-clicked node is editable
            if (e.Node != null && e.Node.Parent != null)
            {
                // Begin editing the node label
                e.Node.BeginEdit();
            }
        }

        private TreeNode CreateTreeNode(XmlNode xmlNode)
        {
            TreeNode treeNode = new TreeNode(xmlNode.Name);

            // Add attributes as child nodes
            if (xmlNode.Attributes != null)
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    TreeNode attributeNode = new TreeNode($"{attribute.Name}: {attribute.Value}");
                    treeNode.Nodes.Add(attributeNode);
                }
            }

            // Recursively add child nodes
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    TreeNode childTreeNode = CreateTreeNode(childNode);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the XML content from the TextBox
                string xmlContent = xmlTextBox.Text;

                // Save the XML content to a file with ISO-8859-2 encoding
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;
                        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding("ISO-8859-2")))
                        {
                            writer.Write(xmlContent);
                        }
                        statusLabel.Text = "XML saved successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XML file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error saving XML file.";
            }
        }
    }
}
