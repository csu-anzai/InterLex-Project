using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models
{
    public class TreeNode
    {
        public string Data { get; set; }
        public Guid Id { get; set; }
        public bool Selectable { get; set; }

        public string Label { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
