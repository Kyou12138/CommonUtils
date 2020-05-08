using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MappingManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MappingManagement.OutputObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.Test
{
    public class NidecMap
    {
        private string MapName { get; set; }
        private int Colums { set; get; } = 0;
        private int Rows { set; get; } = 0;
        private string[,] mapArr;

        public NidecMap(string[,] BinCodeArr, string mapName)
        {
            Rows = BinCodeArr.GetLength(0);
            Colums = BinCodeArr.GetLength(1);
            mapArr = BinCodeArr;
            MapName = mapName;
            CreateNidecMap();
        }

        public void CreateNidecMap()
        {
            try
            {

                if (Rows <= 0 || Colums <= 0)
                {
                    return;
                }

                //从MapDefinition中拿到bioptro的一个定义好的Definition,目的是为了利用已经创建好的filters
                GetObjectByNameOutput nidecMapDefinitionOutput = null;
                GetObjectByNameInput nidecMapDefinitionInput = null;
                MapDefinition nidecMapDefinition = null;
                GetMapDefinitionLayersOutput getMapDefinitionLayersOutput = null;

                try
                {
                    nidecMapDefinitionInput = new GetObjectByNameInput()
                    {
                        Name = string.Format("nidec{0}x{1}", Rows, Colums),//MapDefinition的命名格式为string.Format("bioptro{0}x{1}",行,列)
                        Type = new MapDefinition(),
                        LevelsToLoad = 3
                    };

                    nidecMapDefinitionOutput = nidecMapDefinitionInput.GetObjectByNameSync();

                    if (nidecMapDefinitionOutput != null)
                    {
                        nidecMapDefinition = nidecMapDefinitionOutput.Instance as MapDefinition;
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex);
                }

                //判断当前获取的模板是否与需要的一致，不一致则先去拿默认的然后新建。新建MapDefinition的命名格式为string.Format("bioptro{0}x{1}",行,列)

                try
                {
                    if (nidecMapDefinition == null)
                    {
                        nidecMapDefinitionOutput = null;
                        nidecMapDefinitionInput = new GetObjectByNameInput()
                        {
                            Name = "nidec",//MapDefinition的命名格式为string.Format("bioptro{0}x{1}",行,列)
                            Type = new MapDefinition(),
                            LevelsToLoad = 3
                        };
                        nidecMapDefinitionOutput = nidecMapDefinitionInput.GetObjectByNameSync();
                        if (nidecMapDefinitionOutput != null)
                        {
                            nidecMapDefinition = nidecMapDefinitionOutput.Instance as MapDefinition;
                        }

                        GetMapDefinitionLayersInput getMapDefinitionLayersInput = new GetMapDefinitionLayersInput()
                        {
                            MapDefinitions = new MapDefinitionCollection() { nidecMapDefinition }
                        };
                        getMapDefinitionLayersOutput = getMapDefinitionLayersInput.GetMapDefinitionLayersSync();
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex);
                }




                MapDefinition newBioptroMapDefinition = null;
                CreateMapDefinitionOutput createMapDefinitionOutput = null;
                try
                {
                    if (nidecMapDefinition != null && getMapDefinitionLayersOutput != null)
                    {
                        newBioptroMapDefinition = new MapDefinition()
                        {
                            Name = string.Format("nidec{0}x{1}", Rows, Colums),
                            Rows = Rows,
                            Columns = Colums,
                            MapDefinitionLayers = getMapDefinitionLayersOutput?.MapDefinitions?[0].MapDefinitionLayers,
                            DefaultLayer = getMapDefinitionLayersOutput?.MapDefinitions?[0].MapDefinitionLayers[0].Name,
                            WidthToHeightAspectRatio = 1,
                            ShowGridlinesByDefault = true,
                            Notch = Notch.Left,
                            DefaultRotation = DefaultRotation.Rotate0,
                            ShowRulerByDefault = true,
                            Type = nidecMapDefinition.Type,
                            MaterialUnits = nidecMapDefinition.MaterialUnits,
                            GridlinesColor = nidecMapDefinition.GridlinesColor,
                            EmptyUnitsColor = nidecMapDefinition.EmptyUnitsColor,
                        };

                        //创建MapDefinition
                        CreateMapDefinitionInput createMapDefinitionInput = new CreateMapDefinitionInput();
                        createMapDefinitionInput.MapDefinition = newBioptroMapDefinition;
                        //createMapDefinitionInput.IsNewDefinition = true;
                        createMapDefinitionOutput = createMapDefinitionInput.CreateMapDefinitionSync();
                        nidecMapDefinition = createMapDefinitionOutput.MapDefinition;
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex);
                }

                //创建Map
                CreateMapOutput NidecMapOutput = null;
                try
                {
                    if (nidecMapDefinition != null)
                    {
                        Map nidecMap = new Map()
                        {
                            Name = this.MapName,
                            MapDefinition = nidecMapDefinition,
                        };

                        CreateMapInput nidecMapInput = new CreateMapInput()
                        {
                            Map = nidecMap
                        };
                        NidecMapOutput = nidecMapInput.CreateMapSync();
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex);
                }


                ChangeMapUnitsForBinInput mappingManagement = new ChangeMapUnitsForBinInput();
                try
                {
                    if (NidecMapOutput != null)
                    {
                        foreach (var layer in NidecMapOutput.Map.MapLayers)
                        {
                            mappingManagement.MapLayer = layer;
                            Dictionary<UnitsInput, string> UnitsValues = new Dictionary<UnitsInput, string>();
                            for (int row = 0; row < Rows; row++)
                            {
                                for (int col = 0; col < Colums; col++)
                                {
                                    UnitsValues.Add(new UnitsInput() { Row = row, Column = col }, mapArr[row, col].ToUpper());
                                }
                            }
                            mappingManagement.UnitsValues = UnitsValues;

                            ChangeMapUnitsForBinOuput changeMapUnitsForBinOuput = mappingManagement.ChangeMapUnitsForBinSync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                //log.Error(ex);
            }
        }
    }
}
