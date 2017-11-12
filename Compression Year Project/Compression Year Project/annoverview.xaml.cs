using System.Windows.Controls;
using QuickGraph;
using System.ComponentModel;

namespace Compression_Year_Project
{
    public partial class Annoverview : Page, INotifyPropertyChanged
    {
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get
            {
                return _graphToVisualize;
            }
            set
            {
                if (!Equals(value, this._graphToVisualize))
                {
                    _graphToVisualize = value;
                    RaisePropChanged("GraphToVisualize");
                }
            }
        }
        public string NumLayers;      

        public string SizeHidden;

        public int nl;

        public int sh;

        public page1 p1;
        public Annoverview(page1 pa1)
        {
            CreateGraphToVisualize(2, 2);

            DataContext = this;

            InitializeComponent();

            NumLayers = numLayers.Text;

            SizeHidden = sizeHidden.Text;

            if (StringValidation(NumLayers) && StringValidation(SizeHidden))
            {
                nl = int.Parse(NumLayers);
                sh = int.Parse(SizeHidden);
            }
            p1 = pa1;
        }

        private bool StringValidation(string str)
        {
            if (str != "")
            {
                int temp;
                int.TryParse(str, out temp);
                if (temp > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        private void CreateGraphToVisualize(int num, int sh)
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();
            int numnodes = (num * sh) + 2;
            string[] vertices = new string[numnodes];
            for(int i =0; i < vertices.Length;i++)
            {
                vertices[i] = i.ToString();
                g.AddVertex(vertices[i]);
            }
            int counter = 1;
            int last = vertices.Length - 1;
            for (int j = 0; j < vertices.Length; j++)
            {
                
                if (j == 0)
                {
                    for (int t = 1; t <= sh; t++)
                    {
                        g.AddEdge(new Edge<object>(vertices[0], vertices[t]));
                    }
                }
                else if (j == last)
                {
                    int lst = numnodes - 1 - sh;
                    for (int a = lst; a < numnodes; a++)
                    {
                        g.AddEdge(new Edge<object>(vertices[a], vertices[last]));
                    }
                }
                else
                {
                    if (counter != num)
                    {
                        int first = (counter * sh) + 1;
                        int lt = sh * (counter + 1);
                        for (int b = first; b <= lt; b++)
                        {
                            g.AddEdge(new Edge<object>(vertices[j], vertices[b]));
                        }
                        if (j == counter * sh)
                        {
                            counter++;
                        }
                       
                    }
                }
            }

            GraphToVisualize = g;
        }
        private void Enter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NumLayers = numLayers.Text;

            SizeHidden = sizeHidden.Text;

            if (StringValidation(NumLayers) && StringValidation(SizeHidden))
            {
                int num = int.Parse(NumLayers);
                int sh = int.Parse(SizeHidden);
                CreateGraphToVisualize(num,sh);
                p1.numlayers = num;
                p1.sizehidden = sh;
            }
        }
    }
}
