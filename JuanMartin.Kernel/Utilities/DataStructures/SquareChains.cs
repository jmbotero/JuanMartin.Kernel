using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class SquareChains
    {
        private readonly DirectedAcyclicGraph<int> digits;
        private const string ChainTerminator = "terminator";
        public int Count { get; set; }

        public SquareChains(int count)
        {
            digits = new DirectedAcyclicGraph<int>();
            Count = count;
            CreateChains();
        }

        /// <summary>
        /// Create chains for starting numbers upto count.
        /// Every starting number will eventually arrive at 1 or 89.   
        /// And chain ends when it gets to either of these.  Chains 
        /// ending in 1 have last  digit 1, otherwise  they end in 89,
        /// these nodes have name "terminator".
        /// Chain numbers are vertices in a directeed acyclic graphx:
        /// where vertices are unique by value.
        /// </summary>
        public void CreateChains()
        {
            for (int number = 2; number <= Count; number++)
            {
                Vertex<int> previous = null, current = null;

                var node = digits.Vertices.FirstOrDefault(v => v.Name == number.ToString());

                if (node == null)
                    previous = digits.AddVertex(number);
                else
                    previous = node;

                int start = number;
                bool foundNodeInGraph, foundInCurrentChain, complete;
                var currentChain = $",{number},";
                do
                {
                    number = UtilityMath.SquareDigits(number);
                    foundNodeInGraph = digits.Contains(number);
                    foundInCurrentChain = currentChain.Contains($",{number},");
                    complete = false;

                    if ((number == 1 || number == 89) && foundInCurrentChain)
                    {
                        // first ccheck if terminator already exists to reuse it
                        var terminatorNodeName = ChainTerminator;// $"{ChainTerminator} of {start}";
                        node = digits.Vertices.FirstOrDefault(v => v.Value == number && v.Name == terminatorNodeName);
                        if (node != null)
                            current = node;
                        else
                            current = digits.AddVertex(new Vertex<int>(number, terminatorNodeName));
                        complete = true;
                    }
                    else if (foundNodeInGraph)
                        current = digits.Vertices.FirstOrDefault(v => v.Name == number.ToString());
                    else
                        current = digits.AddVertex(new Vertex<int>(number));

                    if (previous != null && current != null)
                    {
                        currentChain += $",{number},";
                        digits.AddEdge(from: previous, to: current, name: "step");
                    }
                    previous = current;
                    current = null;

                    if (complete)
                        number = start;
                } while (!complete);
            }
        }

        public List<int> GetChain(int number)
        {
            var chain = new List<int>();
            if(number>=2)
            { 
            var start = digits.Vertices.FirstOrDefault(v => v.Value == number);

                if (start != null)
                {
                    var next = start;
                    while (next != null)
                    {
                        var value = next.Value;

                        chain.Add(value);

                        // if we got to the terminator then stop
                        var terminatorNodeName = ChainTerminator;// $"{ChainTerminator} of {start.Value}";

                        if (next.Name == terminatorNodeName)
                            break;

                        // first identify if we got to the  terminator of the chain,
                        // if not just go to   next step.                   
                        var edge = next.Edges.FirstOrDefault(e => (e.Name.Contains(terminatorNodeName) && e.Type == Edge<int>.EdgeType.outgoing));
                        if (edge == null)
                            edge = next.Edges.FirstOrDefault(e => (e.Name.Contains("(step)") && e.Type == Edge<int>.EdgeType.outgoing));

                        if (edge == null)
                            break;
                        next = edge.To;
                    }
                }
                else
                    throw new ArgumentException("Square digit chains  are only defined for numbers greater or equal two.");

                return chain;
            }

            return null;
        }
    }

}
