using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Novinichka.Data.Models;
using Novinichka.Services.NewsSources.Sources;

namespace Novinichka.Data.Seeding
{
    public class SourcesSeeder : ISeeder
    {
        private const string SourceTypeNamePath = "Novinichka.Services.NewsSources.Sources.";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var sources = new List<(string TypeName, string ShortName, string Name, string Description, string Url, string DefaultImageUrl)>
                        {
                            ($"{SourceTypeNamePath}{nameof(AcfBg)}",
                            "АКФ",
                            "Антикорупционен фонд",
                            "„Антикорупционен фонд“ е неправителствена организация, която работи в интерес на гражданското общество и в съдействие на правосъдието за възпрепятстване, изобличаване и разследване на корупцията на всички нива в страната и във високите етажи на властта.",
                            "https://www.mvr.bg/",
                            "/images/sources/mvr.bg.jpg"),
                            ($"{SourceTypeNamePath}{nameof(ApiBg)}",
                            "АПИ",
                            "Агенция „Пътна инфраструктура“",
                            "Агенция „Пътна инфраструктура“ е специализирана агенция към Министерството на регионалното развитие и благоустройството, отговаряща за републиканската пътна мрежа.",
                            "https://www.mvr.bg/",
                            "/images/sources/mvr.bg.jpg"),
                            ($"{SourceTypeNamePath}{nameof(BfuBg)}",
                            "БФС",
                            "Български Футболен Съюз",
                            "Българският футболен съюз е сдружение на футболните клубове в България.",
                            "https://www.mvr.bg/",
                            "/images/sources/mvr.bg.jpg"),
                        };

            foreach (var source in sources)
            {
                var dbSource = dbContext.Sources.FirstOrDefault(x => x.TypeName == source.TypeName);
                if (dbSource == null)
                {
                    dbContext.Sources.Add(
                        new Source
                        {
                            TypeName = source.TypeName,
                            ShortName = source.ShortName,
                            Name = source.Name,
                            Description = source.Description,
                            Url = source.Url,
                        });
                }
                else
                {
                    dbSource.ShortName = source.ShortName;
                    dbSource.Name = source.Name;
                    dbSource.Description = source.Description;
                    dbSource.Url = source.Url;
                }
            }
        }
    }
}
