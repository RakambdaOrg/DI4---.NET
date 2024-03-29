﻿using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Window to enter the data of a subfamily
    /// </summary>
    public partial class AddSubFamily : Form
    {
        /// <summary>
        /// Represents an item with a hidden value in a combobox
        /// For use when an ID is needed (Sub family and brand select)
        /// </summary>
        private class ComboBoxItem
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public override string ToString() { return Name; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize an empty window to add a subfamily
        /// </summary>
        public AddSubFamily()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a window to modify the given subfamily
        /// </summary>
        /// <param name="SubFamily">The subfamily to edit</param>
        public AddSubFamily(SubFamily SubFamily)
        {
            InitializeComponent();
            Construct(SubFamily);
        }

        /// <summary>
        /// Put the attributes of the given subfamily in the corresponding fields
        /// </summary>
        /// <param name="SubFamily">The subfamily (or null for empty window)</param>
        private void Construct(SubFamily SubFamily)
        {
            //Do not allow resizing
            StartPosition = FormStartPosition.CenterParent;
            DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            // Filling the combobox
            SQLiteConnection Connection = DbConnect.GetInstance().GetConnection();
            SQLiteCommand CommandSelectBrands = new SQLiteCommand("SELECT * FROM Familles", Connection);
            SQLiteDataReader ResultFamilies = CommandSelectBrands.ExecuteReader();
            if (ResultFamilies != null)
            {
                while (ResultFamilies.Read())
                {
                    object ObjId = ResultFamilies["RefFamille"];
                    long BrandId = 0;
                    if (ObjId != DBNull.Value)
                    {
                        BrandId = Convert.ToInt64(ObjId);
                    }
                    object ObjName = ResultFamilies["Nom"];
                    string BrandName = "";
                    if (ObjName != DBNull.Value)
                    {
                        BrandName = Convert.ToString(ObjName);
                    }
                    ComboBoxFamily.Items.Add(new ComboBoxItem { Name = BrandName, Value = BrandId });
                }
                ResultFamilies.Close();
            }
            else
            {
                throw new FieldAccessException("Getting families failed");
            }

            if (SubFamily != null)
                SetSubFamily(SubFamily);
            else
                _Id = -1;
        }

        /// <summary>
        /// Return the subfamily created (or edited) by the window
        /// </summary>
        /// <returns>The new subfamily</returns>
        public SubFamily GetSubFamily()
        {
            ComboBoxItem FamilyItem = (ComboBoxItem)ComboBoxFamily.SelectedItem;
            SubFamily SubFamily = new SubFamily(_Id, FamilyItem.Value, TextBoxName.Text);
            return SubFamily;
        }

        /// <summary>
        /// Edit the given subfamily with the new data
        /// </summary>
        /// <param name="SubFamily">The subfamily to edit</param>
        public void SetSubFamily(SubFamily SubFamily)
        {
            for (int BrandIndex = 0; BrandIndex < ComboBoxFamily.Items.Count; BrandIndex++)
            {
                if (((ComboBoxItem)(ComboBoxFamily.Items[BrandIndex])).Value == SubFamily.FamilyReference)
                {
                    ComboBoxFamily.SelectedIndex = BrandIndex;
                    break;
                }
            }
            TextBoxName.Text = SubFamily.Name;
            _Id = SubFamily.Reference;
        }

        /// <summary>
        /// Validate the creation (or editing)
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonOK_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.OK;
            SubFamily SubFamily = GetSubFamily();
            if (SubFamily != null)
                DbConnect.GetInstance().UpdateOrCreateSubFamily(SubFamily);
            Close();
        }

        /// <summary>
        /// Close the window without saving the changes
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonCancel_Click(object Sender, EventArgs Event)
        {
            Close();
        }

        private long _Id;
    }
}
