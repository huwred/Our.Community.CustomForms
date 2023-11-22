using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Packaging;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Implement;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Packaging;
using Umbraco.Extensions;
using static Umbraco.Cms.Core.PropertyEditors.BlockGridConfiguration;

namespace Our.Community.CustomForm.Migrations;


    public class CustomPackageMigrationPlan : PackageMigrationPlan
    {
        public CustomPackageMigrationPlan() : base("Our.Community.CustomForm")
        {
        }

        protected override void DefinePlan()
        {
            To<CustomPackageMigration>(new Guid("ECAC57C0-1217-4506-9BBC-7E164048CBE9"));
        }
    }

    public class CustomPackageMigration : PackageMigrationBase
    {
        public CustomPackageMigration(IPackagingService packagingService, IMediaService mediaService, MediaFileManager mediaFileManager, MediaUrlGeneratorCollection mediaUrlGenerators, IShortStringHelper shortStringHelper, IContentTypeBaseServiceProvider contentTypeBaseServiceProvider, IMigrationContext context, IOptions<PackageMigrationSettings> packageMigrationsSettings) : base(packagingService, mediaService, mediaFileManager, mediaUrlGenerators, shortStringHelper, contentTypeBaseServiceProvider, context, packageMigrationsSettings)
        {
        }


        protected override void Migrate()
        {
            ImportPackage.FromEmbeddedResource(GetType()).Do();
            Context.AddPostMigration<PublishFormBlocksPostMigration>();

        }
    }

