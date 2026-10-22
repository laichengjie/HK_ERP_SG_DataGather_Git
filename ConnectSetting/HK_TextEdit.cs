using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System.ComponentModel;
using System.Windows.Forms;

namespace ConnectSetting
{
    /// <summary>
    /// 
    /// </summary>
    [UserRepositoryItem("RegisterHK_TextEdit")]
    public class RepositoryItemHK_TextEdit : RepositoryItemTextEdit
    {
        static RepositoryItemHK_TextEdit()
        {
            RegisterHK_TextEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        public const string CustomEditName = "HK_TextEdit";

        /// <summary>
        /// 
        /// </summary>
        public RepositoryItemHK_TextEdit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override string EditorTypeName
        {
            get
            {
                return CustomEditName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RegisterHK_TextEdit()
        {
            EditorClassInfo editorClassInfo = CreateClassInfoHK_TextEdit();
            EditorRegistrationInfo.Default.Editors.Add(editorClassInfo);
        }

        protected static EditorClassInfo CreateClassInfoHK_TextEdit()
        {
            EditorClassInfo editorClassInfo = new EditorClassInfo(
                CustomEditName,
                typeof(HK_TextEdit),
                typeof(RepositoryItemHK_TextEdit),
                typeof(TextEditViewInfo),
                new TextEditPainter(),
                true,
                EditImageIndexes.TextEdit,
                typeof(HK_TextEditAccessible) //typeof(DevExpress.Accessibility.TextEditAccessible)
                );

            return editorClassInfo;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ToolboxItem(true)]
    public class HK_TextEdit : TextEdit
    {
        static HK_TextEdit()
        {
            RepositoryItemHK_TextEdit.RegisterHK_TextEdit();
        }

        /// <summary>
        /// 
        /// </summary>
        public HK_TextEdit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override BaseAccessible CreateAccessibleInstance()
        {
            return new HK_TextAccessible(this.Properties);
        }

        /// <summary>
        /// 
        /// </summary>
        [Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemHK_TextEdit Properties
        {
            get
            {
                RepositoryItemHK_TextEdit properties = base.Properties as RepositoryItemHK_TextEdit;
                return properties;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string EditorTypeName
        {
            get
            {
                return RepositoryItemHK_TextEdit.CustomEditName;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HK_TextAccessible : TextAccessible
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public HK_TextAccessible(RepositoryItemTextEdit item) : base(item)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AccessibleStates GetState()
        {
            //if (Item.PasswordChar != 0) return AccessibleStates.Protected;
            AccessibleStates res = base.GetState();
            res = UpdateReadonlyState(res);
            if (Edit.ContainsFocus) res |= AccessibleStates.Focused;
            return res;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HK_TextEditAccessible : TextEditAccessible
    {
        public HK_TextEditAccessible(RepositoryItem item) : base(item)
        {

        }

        protected override AccessibleStates GetState()
        {
            //if (Item.PasswordChar != 0) return AccessibleStates.Protected;
            //if (IsSubstAsEmpty) return AccessibleStates.None;
            return base.GetState();
        }

        protected override TextAccessible CreateTextAccessible()
        {
            return new HK_TextAccessible(Item);
        }
    }
}
