using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Novinichka.Data.Models;
using Novinichka.Data.Seeding.Models;
using Novinichka.Services.NewsSources.Sources;

namespace Novinichka.Data.Seeding;

public class SourcesSeeder : ISeeder
{
    private const string SourceTypeNamePath = "Novinichka.Services.NewsSources.Sources.";

    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        var sources = new List<SourceModel>
        {
            new()
            {
                TypeName = $"{SourceTypeNamePath}{nameof(AcfBg)}",
                ShortName = "АКФ",
                Name = "Антикорупционен фонд",
                Description = "„Антикорупционен фонд“ е неправителствена организация, която работи в интерес на гражданското общество и в съдействие на правосъдието за възпрепятстване, изобличаване и разследване на корупцията на всички нива в страната и във високите етажи на властта.",
                Url = "https://www.acf.bg/",
                DefaultImagePath = "/images/sources/acf.bg.png",
            },
            new()
            {
                TypeName = $"{SourceTypeNamePath}{nameof(ApiBg)}",
                ShortName = "АПИ",
                Name = "Агенция „Пътна инфраструктура“",
                Description = "Агенция „Пътна инфраструктура“ е специализирана агенция към Министерството на регионалното развитие и благоустройството, отговаряща за републиканската пътна мрежа.",
                Url = "https://www.api.bg/",
                DefaultImagePath = "/images/sources/api.bg.jpg",
            },
            new()
            {
                TypeName = $"{SourceTypeNamePath}{nameof(BfuBg)}",
                ShortName = "БФС",
                Name = "Български Футболен Съюз",
                Description = "Българският футболен съюз е сдружение на футболните клубове в България.",
                Url = "https://www.bfunion.bg/",
                DefaultImagePath = "/images/sources/bfunion.bg.png",
            },
        };

        foreach (var source in sources)
        {
            var dbSource = dbContext
                .Sources
                .FirstOrDefault(x => x.TypeName == source.TypeName);

            if (dbSource == null)
            {
                this.AddNewSource(dbContext, source);
            }
            else
            {
                this.UpdateExistingSource(dbContext, source, dbSource);
            }

            await dbContext.SaveChangesAsync();
        }
    }

    private void AddNewSource(ApplicationDbContext dbContext, SourceModel source)
    {
        dbContext.Sources.Add(new Source
        {
            TypeName = source.TypeName,
            ShortName = source.ShortName,
            Name = source.Name,
            Description = source.Description,
            Url = source.Url,
            DefaultImagePath = source.DefaultImagePath,
        });
    }

    private void UpdateExistingSource(ApplicationDbContext dbContext, SourceModel source, Source dbSource)
    {
        dbSource.ShortName = source.ShortName;
        dbSource.Name = source.Name;
        dbSource.Description = source.Description;
        dbSource.Url = source.Url;
        dbSource.DefaultImagePath = source.DefaultImagePath;

        dbContext.Update(dbSource);
    }
}
