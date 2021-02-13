using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class Recipe
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string SpriteName { get; set; }

        public float SecondsToBuild { get; set; }

        public Type StructureClass { get; set; }

        public List<MaterialQuantity> Cost { get; set; }
    }
}
