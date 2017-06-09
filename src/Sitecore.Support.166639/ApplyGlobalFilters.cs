
namespace Sitecore.Support.ContentSearch.Pipelines.GetGlobalFilters
{
    using System.Linq;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq.Extensions;
    using Sitecore.ContentSearch.Pipelines.GetGlobalFilters;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.ContentSearch.Utilities;


    [UsedImplicitly]
    public class ApplyGlobalFilters : GetGlobalFiltersProcessor
    {
        private readonly IContentSearchConfigurationSettings settings;

        public ApplyGlobalFilters()
        {
            this.settings = ContentSearchManager.Locator.GetInstance<IContentSearchConfigurationSettings>();
        }

        public ApplyGlobalFilters(IContentSearchConfigurationSettings settings)
        {
            this.settings = settings;
        }

        public override void Process(GetGlobalFiltersArgs args)
        {
            if (!args.QueryElementType.IsAssignableTo(typeof(UISearchResult)))
            {
                return;
            }

            if (args.QueryElementType == typeof(UISearchResult))
            {
                args.Query = this.GetQueryable<UISearchResult>(args);
            }
            else if (args.QueryElementType == typeof(SitecoreUISearchResultItem))
            {
                args.Query = this.GetQueryable<SitecoreUISearchResultItem>(args);
            }
        }

        private IQueryable<T> GetQueryable<T>(GetGlobalFiltersArgs args) where T : ISearchResult
        {
            IQueryable<T> query = (IQueryable<T>)args.Query;

            string processGuiDs = IdHelper.ProcessGUIDs(args.StartLocationItem != null ? args.StartLocationItem.Id.ToString() : ItemIDs.RootID.ToString(), true);
            if (args.RestrictPath)
            {
                query = query.Where(i => i["_path"] == processGuiDs);
            }

            query = query.Where(i => i["_latestversion"] == "1");
            query = query.Where(i => i["_datasource"] == "sitecore");

            if (this.settings.ExcludeContextItemFromResult())
            {
                query = query.Where(i => i["_group"] != processGuiDs);
            }

            return query;
        }
    }
}