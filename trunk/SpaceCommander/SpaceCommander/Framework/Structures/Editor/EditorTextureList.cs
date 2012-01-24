#if !XBOX360
namespace GameApplicationTools.Structures.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using Microsoft.Xna.Framework.Graphics;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using System.Windows.Forms;
    using GameApplicationTools.Actors;


    public class EditorTextureList : UITypeEditor
    {
        IWindowsFormsEditorService editorService;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ListBox listBox = new ListBox();
            listBox.Width = 300;
            foreach (string key in ResourceManager.Instance.GetResourcesOfType<Texture2D>().Keys)
            {
                listBox.Items.Add(key);
            }


            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            editorService.DropDownControl(listBox);

            listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);

            if (listBox.SelectedItem != null)
            {
                string item = "default";
                item = listBox.SelectedItem.ToString();

                return ResourceManager.Instance.GetResource<Texture2D>(item);
            }

            return listBox.SelectedItem;
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            editorService.CloseDropDown();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
#endif