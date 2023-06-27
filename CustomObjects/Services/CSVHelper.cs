using System.IO;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using CsvHelper.Configuration;
using CustomObjects.Models;
using MetadataAPI;
using System.Linq;
using CsvHelper.TypeConversion;

namespace CustomObjects.Services
{
    public class CSVHelper
    {



        public List<HubSpotModel> ParseCsvFile(string filePath)
        {
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null,

            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<HubSpotModelMap>();
                var records = new List<HubSpotModel>();

                records = csv.GetRecords<HubSpotModel>().ToList();

                return records;
            }
        }
        public class HubSpotModelMap : ClassMap<HubSpotModel>

        {

            public HubSpotModelMap()
            {
                {
                    Map(m => m.Name).Name("Name");
                    Map(m => m.InternalName).Name("Internal name");
                    Map(m => m.Type).Name("Type");
                    Map(m => m.Description).Name("Description");

                }
            }

        }

    }

}


