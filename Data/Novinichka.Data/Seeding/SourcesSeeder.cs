using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Novinichka.Data.Models;

namespace Novinichka.Data.Seeding
{
    public class SourcesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var sources =
                new List<(string TypeName, string ShortName, string Name, string Description, string Url, string
                    DefaultImageUrl)>
                {
                    ("Novinichka.Services.NewsSources.Sources.BnbBg", "БНБ",
                        "Българска народна банка",
                        "Българската народна банка (БНБ) е централната банка на Република България. Българска народна банка е основана на 25 януари 1879 г. Основна цел на Българската народна банка е да поддържа ценовата стабилност чрез осигуряване стабилността на националната парична единица и провеждане на парична политика в съответствие с изискванията на закона.",
                        "https://bnb.bg/",
                        "/images/sources/bnb.bg.jpg"),
                    ("Novinichka.Services.NewsSources.Sources.VksBg", "ВКС",
                        "Върховен касационен съд",
                        "Върховният касационен съд (ВКС) на България е висшата съдебна инстанция в България. Осъществява върховен съдебен надзор върху гражданските, наказателните, военните и търговските дела и налага точно и еднакво прилагане на законите от всички съдилища. Решенията на Върховния касационен съд са окончателни и не подлежат на обжалване. В него днес работят 85 съдии с над 15 години юридически стаж, разпределени в три колегии: гражданска, наказателна и търговска. ВКС разглежда жалби и протести срещу актове на окръжните съдилища, когато същите са действали като втора инстанция, и срещу актове на апелативните съдилища. При възникване на въпрос за конституционосъобразност, съдът може да го отнесе до Конституционния съд.",
                        "http://vks.bg/",
                        "/images/sources/vks.bg.jpg"),
                };

            foreach (var source in sources)
            {
                var dbSource = dbContext
                    .Sources
                    .FirstOrDefault(x => x.TypeName == source.TypeName);

                if (dbSource == null)
                {
                    var sourceToAdd = new Source
                    {
                        TypeName = source.TypeName,
                        ShortName = source.ShortName,
                        Name = source.Name,
                        Description = source.Description,
                        Url = source.Url,
                        DefaultImageUrl = source.DefaultImageUrl,
                    };

                    await dbContext.Sources.AddAsync(sourceToAdd);
                }
                else
                {
                    dbSource.ShortName = source.ShortName;
                    dbSource.Name = source.Name;
                    dbSource.Description = source.Description;
                    dbSource.Url = source.Url;
                    dbSource.DefaultImageUrl = source.DefaultImageUrl;
                }
            }
        }
    }
}
