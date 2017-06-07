using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectServer.Client;
using Project.Performance.Model;
using Project.Performance.Utility;

namespace Project.Performance.Repository
{
    public class LookupTableCSOMClient : PerformanceBaseClient, ICSOMClient<LookupTableModel>
    {
        #region Constructors
        #endregion

        #region ICSOMClient
        public bool Create(LookupTableModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("The LookupTableModel is null.");
            }
            else
            {
                if (GetEntities().Any(item => item.Name == model.Name))
                {
                    // No need to do anything currently
                }
                else
                {
                    IEnumerable<LookupEntryCreationInformation> lstLookupEntry = CreateLookupEntry(model);
                    IEnumerable<LookupMask> lstLookupMask = CreateLookupMask(model);
                    LookupTableCreationInformation newLookupTable = new LookupTableCreationInformation()
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        SortOrder = LookupTableSortOrder.Ascending,
                        Masks = lstLookupMask,
                        Entries = lstLookupEntry
                    };
                    bool result = ExcuteJobWithRetries(() =>
                    {
                        LoggerHelper.Instance.Comment("About to Create LookupTable with Name: " + newLookupTable.Name);
                        LookupTable table = BaseProjectContext.LookupTables.Add(newLookupTable);
                        BaseProjectContext.LookupTables.Update();
                        BaseProjectContext.ExecuteQuery();
                        LoggerHelper.Instance.Comment("Finish Creating LookupTable with Name: " + newLookupTable.Name);
                    },
                    "Create LookupTable");
                    return result;
                }
            }
            return true;
        }

        public bool Delete(LookupTableModel model)
        {
            throw new NotImplementedException("LookupTableModel Delete");
        }

        public int GetCount(XMLModel model)
        {
            if (model != null && model.LookupTableModelList != null)
            {
                return GetEntities().Count(item => model.LookupTableModelList.Any(obj => obj.Name == item.Name));
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<LookupTable> GetEntities()
        {
            IEnumerable<LookupTable> lookuptables = null;
            ExcuteJobWithRetries(() =>
            {
                lookuptables = BaseProjectContext.LoadQuery(BaseProjectContext.LookupTables);
                BaseProjectContext.ExecuteQuery();
            },
            "Query LookupTable");
            return lookuptables ?? new List<LookupTable>();
        }
        #endregion

        #region Private Methods
        public IEnumerable<LookupEntryCreationInformation> CreateLookupEntry(LookupTableModel model)
        {
            if (model.Type.ToLower() == "text")
            {
                // Save different level lookup entry: lstEntry[0,1,...model.NumberOfValues-1], the index = level - 1
                List<List<LookupEntryCreationInformation>> lstLookupEntry = new List<List<LookupEntryCreationInformation>>();
                for (int i = 0; i < model.NumberOfValues; i++)
                {
                    int levelDepth = RandomHelper.Random(0, Math.Min(model.OutlineLevels - 1, lstLookupEntry.Count));
                    Guid parentId = levelDepth == 0 ? default(Guid) : lstLookupEntry[levelDepth - 1][RandomHelper.Random(0, lstLookupEntry[levelDepth - 1].Count - 1)].Id;
                    if (levelDepth == lstLookupEntry.Count)
                    {
                        lstLookupEntry.Add(new List<LookupEntryCreationInformation>() { NewTextLookupEntry(levelDepth, parentId) });
                    }
                    else
                    {
                        lstLookupEntry[levelDepth].Add(NewTextLookupEntry(levelDepth, parentId));
                    }
                }
                return lstLookupEntry.SelectMany(item => item).ToList();
            }
            else
            {
                List<LookupEntryCreationInformation> lstEntry = new List<LookupEntryCreationInformation>();
                for (int i = 0; i < model.NumberOfValues; i++)
                {
                    LookupEntryValue entityValue = null;
                    switch (model.Type.ToLower())
                    {
                        case "number":
                            entityValue = new LookupEntryValue() { NumberValue = i };
                            break;
                        case "date":
                            entityValue = new LookupEntryValue() { DateValue = DateTime.Now.AddDays(i) };
                            break;
                        case "duration":
                            entityValue = new LookupEntryValue() { DurationValue = i.ToString() };
                            break;
                        default:
                            entityValue = new LookupEntryValue() { NumberValue = i };
                            break;
                    }

                    lstEntry.Add(new LookupEntryCreationInformation()
                    {
                        Id = Guid.NewGuid(),
                        Value = entityValue
                    });
                }
                return lstEntry;
            }
        }

        public IEnumerable<LookupMask> CreateLookupMask(LookupTableModel model)
        {
            LookupTableMaskSequence maskType = LookupTableMaskSequence.CHARACTERS;
            switch (model.Type.ToLower())
            {
                case "text":
                    maskType = LookupTableMaskSequence.CHARACTERS;
                    break;
                case "number":
                    maskType = LookupTableMaskSequence.NUMBER_DECIMAL;
                    break;
                case "cost":
                    maskType = LookupTableMaskSequence.COST;
                    break;
                case "duration":
                    maskType = LookupTableMaskSequence.DURATION;
                    break;
                case "date":
                    maskType = LookupTableMaskSequence.DATE;
                    break;
                default:
                    throw new ArgumentException("The type of lookup table is illegal.");
            }

            List<LookupMask> lstLookupMask = new List<LookupMask>();
            if (model.Type.ToLower() == "text")
            {
                for (int i = 0; i < model.OutlineLevels; i++)
                {
                    LookupMask mask = new LookupMask()
                    {
                        MaskType = maskType,
                        Separator = ".",
                    };
                    lstLookupMask.Add(mask);
                }
            }
            else
            {
                LookupMask mask = new LookupMask()
                {
                    Length = 0,
                    Separator = ".",
                    MaskType = maskType
                };
                lstLookupMask.Add(mask);
            }
            return lstLookupMask;
        }

        private LookupEntryCreationInformation NewTextLookupEntry(int value, Guid parentId)
        {
            return new LookupEntryCreationInformation()
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                Value = new LookupEntryValue() { TextValue = value.ToString() + Guid.NewGuid() }
            };
        }
        #endregion
    }
}
