using System.ComponentModel;

namespace CustomObjects.Models
{
    public class HubSpotModel
    {

        public HubSpotModel() // Parameterless constructor
        {
        }



            public string Name { get; set; }
            public string InternalName { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }


    }
}
