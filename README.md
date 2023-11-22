# Our.Cummunity.CustomForm
To use this package you need to be using the UmBootstrap template for Umbraco.

The package installs some document types to enable the creation of custom forms for UmBootstrap using the BlockGrid.
![Example custom form](images/customform.png)

## After Installing the package
After the package has installed, there are a few steps you need to perfom to get everything working.

1. You will need to create a SurfaceController to accept and process your custom form when it is submitted.

``` public class TestSurfaceController : SurfaceController
    {
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        public TestSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider,
            Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SubmitTestForm([FromForm] IFormCollection formModel)
        {
            if (formModel.Files.Any())
            {
                //handle the file upload
                foreach (var file in formModel.Files)  
                {  
                    if (file.Length > 0)  
                    {  
                        var infile = Path.Combine(_hostingEnvironment.ApplicationPhysicalPath, @"wwwroot", formModel["FormControlUploadPath"], file.FileName);  
  
                        using (var stream = System.IO.File.Create(infile))  
                        {  
                            file.CopyToAsync(stream);  
                        }  
                    }  
                }  
            }
            return View("TestResult",formModel);
        }
    }```

2. Add the line below to your _ViewImports.cshtml

    ```@addTagHelper *, Our.Umbraco.TagHelpers```

3. tt

## Creating a Form
