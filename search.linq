<Query Kind="Program">
  <NuGetReference>NuGet.PackageManagement</NuGetReference>
  <NuGetReference>NuGet.Packaging</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>NuGet.Protocol.Core.Types</Namespace>
  <Namespace>NuGet.Configuration</Namespace>
  <Namespace>NuGet.ProjectManagement</Namespace>
  <Namespace>NuGet.PackageManagement</Namespace>
</Query>

NuGet.Protocol.Core.Types.SourceRepository sourceRepository;

async Task Main()
{
	InitializeNugetDependencies();
	var jsonNugetPackages = await SearchTestPackage("Newtonsoft.Json");
	jsonNugetPackages.Dump();
}

private void InitializeNugetDependencies() 
{
	List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
	providers.AddRange(Repository.Provider.GetCoreV3());

	PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
	this.sourceRepository = new NuGet.Protocol.Core.Types.SourceRepository(packageSource, providers);

	var rootPath = @"yourPathToNugetFolder"; // where packages will be installed
	var settings = Settings.LoadDefaultSettings(rootPath);
	var packageSourceProvider = new PackageSourceProvider(settings);
	var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);

	var project = new FolderNuGetProject(rootPath);
	var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, rootPath)
	{
		PackagesFolderNuGetProject = project
	};
}


private async Task<IEnumerable<IPackageSearchMetadata>> SearchTestPackage(string packageName)
{
	var searchResource = await this.sourceRepository.GetResourceAsync<PackageSearchResource>();
	var supportedFramework = new[] { ".NETFramework,Version=v4.6" };
	var searchFilter = new SearchFilter(true)
	{
		SupportedFrameworks = supportedFramework,
		IncludeDelisted = false
	};

	return await searchResource
		.SearchAsync(packageName, searchFilter, 0, 10, null, CancellationToken.None);
}