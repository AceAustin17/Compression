using System.Windows.Controls;
using QuickGraph;
using System.ComponentModel;

namespace Compression_Year_Project
{
    public partial class Annoverview : Page
    {        
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;

        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get
            {
                return _graphToVisualize;
            }
        }
        public Annoverview()
        {
            CreateGraphToVisualize();

            DataContext = this;

            InitializeComponent();
        }

        private void CreateGraphToVisualize()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            string[] vertices = new string[5];
            for(int i =0; i < vertices.Length;i++)
            {
                vertices[i] = i.ToString();
                g.AddVertex(vertices[i]);
            }

            g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[4]));

            _graphToVisualize = g;
        }
    }
}
