using System.Windows.Forms;

namespace ShoeVox
{
    public partial class SetPrefixDialog : Form
    {
        public string GetPrefix
        {
            get
            {
                return prefixText.Text;
            }
        }

        public SetPrefixDialog(string currentPrefix)
        {
            InitializeComponent();
            prefixText.Text = currentPrefix;
        }
    }
}
