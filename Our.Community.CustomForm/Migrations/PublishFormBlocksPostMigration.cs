using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using static Umbraco.Cms.Core.PropertyEditors.BlockGridConfiguration;

namespace Our.Community.CustomForm.Migrations
{
    public class PublishFormBlocksPostMigration : MigrationBase
    {
        private readonly ILogger<CustomPackageMigration> _logger;
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly PropertyEditorCollection _propertyEditorCollection;
        public PublishFormBlocksPostMigration(
            ILogger<CustomPackageMigration> logger,
            IDataTypeService dataTypeService,
            IContentTypeService contentTypeService,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            PropertyEditorCollection propertyEditorCollection,
            IMigrationContext context) : base(context)
        {
            _logger = logger;

            _dataTypeService = dataTypeService;
            _contentTypeService = contentTypeService;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
            _propertyEditorCollection = propertyEditorCollection;
        }

        protected override void Migrate()
        {
            var grid = _dataTypeService.GetDataType(Guid.Parse("9490ca3c-7a5e-4be3-aa73-712136b2a5a1"));
            if (grid == null)
            {
                //doesn't look like they are using umBootstrap
                return;
            }

            AddCustomFormLayoutBlock(grid);
            AddCustomFormControlBlocks(grid);
        }
        private void AddCustomFormControlBlocks(IDataType grid)
        {
            BlockGridConfiguration blockGridDataTemp = (BlockGridConfiguration)grid.Configuration;
            var controlsGroup = blockGridDataTemp?.BlockGroups.FirstOrDefault(x => x.Name == "Form Controls");
            List<string> formcontrols = new List<string>()
                {
                    "formControlCheckbox",
                    "formControlDateTime",
                    "formControlList",
                    "formControlRadioButton",
                    "formControlRichText",
                    "formControlTextArea",
                    "formControlTextInput",
                    "formControlUpload",
                    "formControlSubmitButton"
                };
            var formcontrolBlocks = new List<BlockGridBlockConfiguration>();
            var colSpanOptions = new List<BlockGridColumnSpanOption>
            {
                new() { ColumnSpan = 4 },
                new() { ColumnSpan = 6 },
                new() { ColumnSpan = 8 },
                new() { ColumnSpan = 10},
                new() { ColumnSpan = 12}
            };
            foreach (var control in formcontrols)
            {
                //get the elements for the block.
                IContentType formControlContentType = _contentTypeService.Get(control);
                IContentType formControlSettingsType = _contentTypeService.Get("formControlSettings");
                if (control == "formControlSubmitButton")
                {
                    formControlSettingsType = _contentTypeService.Get("formSubmitSettings");
                }

                var newblockdata = new BlockGridBlockConfiguration()
                {
                    AllowAtRoot = false,
                    AllowInAreas = true,
                    EditorSize = "medium",
                    RowMaxSpan = 1,
                    RowMinSpan = 1,
                    Label = "",
                    ContentElementTypeKey = formControlContentType.Key,
                    SettingsElementTypeKey = formControlSettingsType.Key,
                    GroupKey = controlsGroup?.Key.ToString(),
                    View = "~/App_Plugins/Umbraco.Community.BlockPreview/views/block-preview.html",
                    Stylesheet = "~/css/index.css",
                    ColumnSpanOptions = colSpanOptions.ToArray()
                    
                };
                formcontrolBlocks.Add(newblockdata);

            }

            try
            {
                CreateBlockGridDataType(formcontrolBlocks, grid);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"CreateBlockGridDataType:");
                throw;
            }
        }

        private void AddCustomFormLayoutBlock(IDataType grid)
        {
            // get the blockgrid groups
            BlockGridConfiguration blockGridDataTemp = (BlockGridConfiguration)grid.Configuration;
            // filter out the block. 
            var layoutGroup = blockGridDataTemp?.BlockGroups.FirstOrDefault(x => x.Name == "Layout Blocks");
            var controlsGroup = blockGridDataTemp?.BlockGroups.FirstOrDefault(x => x.Name == "Form Controls");
            Guid groupkey;
            if (controlsGroup == null)
            {
                var formControlsgroup = new BlockGridGroupConfiguration()
                {
                    Key = Guid.NewGuid(),
                    Name = "Form Controls"
                };
                var newgroups = ((BlockGridConfiguration)grid.Configuration)?.BlockGroups.Append(formControlsgroup);
                if (newgroups != null) ((BlockGridConfiguration)grid.Configuration)!.BlockGroups = newgroups.ToArray();
                groupkey = formControlsgroup.Key;
            }
            else
            {
                groupkey = controlsGroup.Key;
            }

            //add an area for the controls
            var area = new BlockGridAreaConfiguration()
            {
                Alias = "formlayout g-col-12 p-3 rounded-3",
                Key = Guid.NewGuid(),
                ColumnSpan = 12,
                MinAllowed = 1,
                RowSpan = 1,
                SpecifiedAllowance = new[]
                {
                    new BlockGridAreaConfigurationSpecifiedAllowance
                    {
                        GroupKey = groupkey
                    }
                }
            };
            var areas = new List<BlockGridAreaConfiguration> { area };
            //get the elements for the block.
            IContentType formContentType = _contentTypeService.Get("formLayout");
            IContentType formSettingsType = _contentTypeService.Get("formSettings");

            var newblockdata = new BlockGridBlockConfiguration()
            {
                AllowAtRoot = true,
                AllowInAreas = true,
                EditorSize = "medium",
                RowMaxSpan = 1,
                RowMinSpan = 1,
                ContentElementTypeKey = formContentType.Key,
                SettingsElementTypeKey = formSettingsType.Key,
                GroupKey = layoutGroup?.Key.ToString(),
                Areas = areas.ToArray()
            };

            try
            {
                CreateBlockGridDataType(new List<BlockGridBlockConfiguration> { newblockdata }, grid);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"CreateBlockGridDataType:");
                throw;
            }
        }

        public void CreateBlockGridDataType(List<BlockGridBlockConfiguration> blocks, IDataType grid)
        {
            var editor = _propertyEditorCollection.First(x => x.Alias == "Umbraco.BlockGrid");

            var existingblocks = ((BlockGridConfiguration)grid.Configuration!).Blocks.ToList();
            //Need to check if they exist already!!
            //var query = blocks.Except(existingblocks).ToList();

            existingblocks.AddRange(blocks);
            (((BlockGridConfiguration)grid.Configuration)!).Blocks = existingblocks.ToArray();


            var newDataType = new DataType(editor, _configurationEditorJsonSerializer)
            {
                Name = grid.Name,
                Configuration = (BlockGridConfiguration)grid.Configuration,
                ParentId = grid.ParentId,
                Key = grid.Key,
                Id = grid.Id,
            };

            _dataTypeService.Save(newDataType);
        }

    }
}