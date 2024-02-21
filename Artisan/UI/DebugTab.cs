﻿using Artisan.Autocraft;
using Artisan.CraftingLists;
using Artisan.CraftingLogic;
using Artisan.CraftingLogic.Solvers;
using Artisan.GameInterop;
using Artisan.GameInterop.CSExt;
using Artisan.IPC;
using Artisan.RawInformation;
using Artisan.RawInformation.Character;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static ECommons.GenericHelpers;

namespace Artisan.UI
{
    internal unsafe class DebugTab
    {
        internal static int offset = 0;
        internal static int SelRecId = 0;
        internal static bool Debug = false;
        public static int DebugValue = 1;

        internal static void Draw()
        {
            try
            {
                ImGui.Checkbox("Debug logging", ref Debug);
                if (ImGui.CollapsingHeader("Crafter's food"))
                {
                    foreach (var x in ConsumableChecker.GetFood())
                    {
                        ImGuiEx.Text($"{x.Id}: {x.Name}");
                    }
                }
                if (ImGui.CollapsingHeader("Crafter's food in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetFood(true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id);
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Crafter's HQ food in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetFood(true, true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id, true);
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Crafter's pots"))
                {
                    foreach (var x in ConsumableChecker.GetPots())
                    {
                        ImGuiEx.Text($"{x.Id}: {x.Name}");
                    }
                }
                if (ImGui.CollapsingHeader("Crafter's pots in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetPots(true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id);
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Crafter's HQ pots in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetPots(true, true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id, true);
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Manuals"))
                {
                    foreach (var x in ConsumableChecker.GetManuals())
                    {
                        ImGuiEx.Text($"{x.Id}: {x.Name}");
                    }
                }
                if (ImGui.CollapsingHeader("Manuals in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetManuals(true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id);
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Squadron Manuals"))
                {
                    foreach (var x in ConsumableChecker.GetSquadronManuals())
                    {
                        ImGuiEx.Text($"{x.Id}: {x.Name}");
                    }
                }
                if (ImGui.CollapsingHeader("SquadronManuals in inventory"))
                {
                    foreach (var x in ConsumableChecker.GetSquadronManuals(true))
                    {
                        if (ImGui.Selectable($"{x.Id}: {x.Name}"))
                        {
                            ConsumableChecker.UseItem(x.Id);
                        }
                    }
                }

                if (ImGui.CollapsingHeader("Crafting Stats") && Crafting.CurCraft != null && Crafting.CurStep != null)
                {
                    ImGui.Text($"Control: {Crafting.CurCraft.StatControl}");
                    ImGui.Text($"Craftsmanship: {Crafting.CurCraft.StatCraftsmanship}");
                    ImGui.Text($"Current Durability: {Crafting.CurStep.Durability}");
                    ImGui.Text($"Max Durability: {Crafting.CurCraft.CraftDurability}");
                    ImGui.Text($"Current Progress: {Crafting.CurStep.Progress}");
                    ImGui.Text($"Max Progress: {Crafting.CurCraft.CraftProgress}");
                    ImGui.Text($"Current Quality: {Crafting.CurStep.Quality}");
                    ImGui.Text($"Max Quality: {Crafting.CurCraft.CraftQualityMax}");
                    ImGui.Text($"Quality Percent: {Calculations.GetHQChance(Crafting.CurStep.Quality * 100.0 / Crafting.CurCraft.CraftQualityMax)}");
                    ImGui.Text($"Item name: {Crafting.CurRecipe?.ItemResult.Value?.Name}");
                    ImGui.Text($"Current Condition: {Crafting.CurStep.Condition}");
                    ImGui.Text($"Current Step: {Crafting.CurStep.Index}");
                    ImGui.Text($"Quick Synth: {Crafting.QuickSynthState.Cur} / {Crafting.QuickSynthState.Max}");
                    ImGui.Text($"GS+ByregotCombo: {StandardSolver.GreatStridesByregotCombo(Crafting.CurCraft, Crafting.CurStep)}");
                    ImGui.Text($"Base Quality: {Simulator.BaseQuality(Crafting.CurCraft)}");
                    ImGui.Text($"Base Progress: {Simulator.BaseProgress(Crafting.CurCraft)}");
                    ImGui.Text($"Predicted Quality: {StandardSolver.CalculateNewQuality(Crafting.CurCraft, Crafting.CurStep, CraftingProcessor.NextRec.Action)}");
                    ImGui.Text($"Predicted Progress: {StandardSolver.CalculateNewProgress(Crafting.CurCraft, Crafting.CurStep, CraftingProcessor.NextRec.Action)}");
                    ImGui.Text($"Collectibility Low: {Crafting.CurCraft.CraftQualityMin1}");
                    ImGui.Text($"Collectibility Mid: {Crafting.CurCraft.CraftQualityMin2}");
                    ImGui.Text($"Collectibility High: {Crafting.CurCraft.CraftQualityMin3}");
                    ImGui.Text($"Crafting State: {Crafting.CurState}");
                    ImGui.Text($"Can Finish: {StandardSolver.CanFinishCraft(Crafting.CurCraft, Crafting.CurStep, CraftingProcessor.NextRec.Action)}");
                    ImGui.Text($"Current Rec: {CraftingProcessor.NextRec.Action.NameOfAction()}");
                    ImGui.Text($"Previous Action: {Crafting.CurStep.PrevComboAction.NameOfAction()}");
                    ImGui.Text($"Can insta delicate: {Crafting.CurStep.Index == 1 && StandardSolver.CanFinishCraft(Crafting.CurCraft, Crafting.CurStep, Skills.DelicateSynthesis) && StandardSolver.CalculateNewQuality(Crafting.CurCraft, Crafting.CurStep, Skills.DelicateSynthesis) >= Crafting.CurCraft.CraftQualityMin3}");
                }

                if (ImGui.CollapsingHeader("Spiritbonds"))
                {
                    ImGui.Text($"Weapon Spiritbond: {Spiritbond.Weapon}");
                    ImGui.Text($"Off-hand Spiritbond: {Spiritbond.Offhand}");
                    ImGui.Text($"Helm Spiritbond: {Spiritbond.Helm}");
                    ImGui.Text($"Body Spiritbond: {Spiritbond.Body}");
                    ImGui.Text($"Hands Spiritbond: {Spiritbond.Hands}");
                    ImGui.Text($"Legs Spiritbond: {Spiritbond.Legs}");
                    ImGui.Text($"Feet Spiritbond: {Spiritbond.Feet}");
                    ImGui.Text($"Earring Spiritbond: {Spiritbond.Earring}");
                    ImGui.Text($"Neck Spiritbond: {Spiritbond.Neck}");
                    ImGui.Text($"Wrist Spiritbond: {Spiritbond.Wrist}");
                    ImGui.Text($"Ring 1 Spiritbond: {Spiritbond.Ring1}");
                    ImGui.Text($"Ring 2 Spiritbond: {Spiritbond.Ring2}");

                    ImGui.Text($"Spiritbond Ready Any: {Spiritbond.IsSpiritbondReadyAny()}");

                }

                if (ImGui.CollapsingHeader("Quests"))
                {
                    QuestManager* qm = QuestManager.Instance();
                    foreach (var quest in qm->DailyQuestsSpan)
                    {
                        ImGui.TextWrapped($"Quest ID: {quest.QuestId}, Sequence: {QuestManager.GetQuestSequence(quest.QuestId)}, Name: {quest.QuestId.NameOfQuest()}, Flags: {quest.Flags}");
                    }

                }

                if (ImGui.CollapsingHeader("IPC"))
                {
                    ImGui.Text($"AutoRetainer: {AutoRetainer.IsEnabled()}");
                    if (ImGui.Button("Suppress"))
                    {
                        AutoRetainer.Suppress();
                    }
                    if (ImGui.Button("Unsuppress"))
                    {
                        AutoRetainer.Unsuppress();
                    }

                    ImGui.Text($"Endurance IPC: {Svc.PluginInterface.GetIpcSubscriber<bool>("Artisan.GetEnduranceStatus").InvokeFunc()}");
                    ImGui.Text($"List IPC: {Svc.PluginInterface.GetIpcSubscriber<bool>("Artisan.IsListRunning").InvokeFunc()}");
                    if (ImGui.Button("Enable"))
                    {
                        Svc.PluginInterface.GetIpcSubscriber<bool, object>("Artisan.SetEnduranceStatus").InvokeAction(true);
                    }
                    if (ImGui.Button("Disable"))
                    {
                        Svc.PluginInterface.GetIpcSubscriber<bool, object>("Artisan.SetEnduranceStatus").InvokeAction(false);
                    }

                    if (ImGui.Button("Send Stop Request (true)"))
                    {
                        Svc.PluginInterface.GetIpcSubscriber<bool, object>("Artisan.SetStopRequest").InvokeAction(true);
                    }

                    if (ImGui.Button("Send Stop Request (false)"))
                    {
                        Svc.PluginInterface.GetIpcSubscriber<bool, object>("Artisan.SetStopRequest").InvokeAction(false);
                    }
                }

                if (ImGui.CollapsingHeader("Collectables"))
                {
                    foreach (var item in LuminaSheets.ItemSheet.Values.Where(x => x.IsCollectable).OrderBy(x => x.LevelItem.Row))
                    {
                        if (Svc.Data.GetExcelSheet<CollectablesShopItem>().TryGetFirst(x => x.Item.Row == item.RowId, out var collectibleSheetItem))
                        {
                            if (collectibleSheetItem != null)
                            {
                                ImGui.Text($"{item.Name} - {collectibleSheetItem.CollectablesShopRewardScrip.Value.LowReward}");
                            }
                        }
                    }
                }

                if (ImGui.CollapsingHeader("RecipeNote"))
                {
                    var recipes = RecipeNoteRecipeData.Ptr();
                    if (recipes != null && recipes->Recipes != null)
                    {
                        if (recipes->SelectedIndex < recipes->RecipesCount)
                            DrawRecipeEntry($"Selected", recipes->Recipes + recipes->SelectedIndex);
                        for (int i = 0; i < recipes->RecipesCount; ++i)
                            DrawRecipeEntry(i.ToString(), recipes->Recipes + i);
                    }
                    else
                    {
                        ImGui.TextUnformatted($"Null: {(nint)recipes:X}");
                    }
                }

                if (ImGui.CollapsingHeader("Gear"))
                {
                    ImGui.TextUnformatted($"In-game stats: {CharacterInfo.Craftsmanship}/{CharacterInfo.Control}/{CharacterInfo.MaxCP}");
                    DrawEquippedGear();
                    foreach (ref var gs in RaptureGearsetModule.Instance()->EntriesSpan)
                        DrawGearset(ref gs);
                }

                if (ImGui.CollapsingHeader("Repairs"))
                {
                    if (ImGui.Button("Repair all"))
                    {
                        RepairManager.ProcessRepair();
                    }
                    ImGuiEx.Text($"Gear condition: {RepairManager.GetMinEquippedPercent()}");

                    ImGui.Text($"Can Repair: {(LuminaSheets.ItemSheet.ContainsKey((uint)DebugValue) ? LuminaSheets.ItemSheet[(uint)DebugValue].Name : "")} {RepairManager.CanRepairItem((uint)DebugValue)}");
                    ImGui.Text($"Can Repair Any: {RepairManager.CanRepairAny()}");
                    ImGui.Text($"Repair NPC Nearby: {RepairManager.RepairNPCNearby(out _)}");

                    if (ImGui.Button("Interact with RepairNPC"))
                    {
                        P.TM.Enqueue(() => RepairManager.InteractWithRepairNPC(), "RepairManagerDebug");
                    }

                    ImGui.Text($"Repair Price: {RepairManager.GetNPCRepairPrice()}");

                }

                ImGui.Separator();

                ImGui.Text($"Endurance Item: {Endurance.RecipeID} {Endurance.RecipeName}");
                if (ImGui.Button($"Open Endurance Item"))
                {
                    CraftingListFunctions.OpenRecipeByID(Endurance.RecipeID);
                }

                ImGui.InputInt("Debug Value", ref DebugValue);
                if (ImGui.Button($"Open Recipe"))
                {
                    AgentRecipeNote.Instance()->OpenRecipeByRecipeId((uint)DebugValue);
                }

                ImGui.Text($"Item Count? {CraftingListUI.NumberOfIngredient((uint)DebugValue)}");

                ImGui.Text($"Completed Recipe? {((uint)DebugValue).NameOfRecipe()} {P.ri.HasRecipeCrafted((uint)DebugValue)}");

                if (ImGui.Button($"Open And Quick Synth"))
                {
                    Operations.QuickSynthItem(DebugValue);
                }
                if (ImGui.Button($"Close Quick Synth Window"))
                {
                    Operations.CloseQuickSynthWindow();
                }
                if (ImGui.Button($"Open Materia Window"))
                {
                    Spiritbond.OpenMateriaMenu();
                }
                if (ImGui.Button($"Extract First Materia"))
                {
                    Spiritbond.ExtractFirstMateria();
                }

                if (ImGui.Button($"Pandora IPC"))
                {
                    var state = Svc.PluginInterface.GetIpcSubscriber<string, bool?>($"PandorasBox.GetFeatureEnabled").InvokeFunc("Auto-Fill Numeric Dialogs");
                    Svc.Log.Debug($"State of Auto-Fill Numeric Dialogs: {state}");
                    Svc.PluginInterface.GetIpcSubscriber<string, bool, object>($"PandorasBox.SetFeatureEnabled").InvokeAction("Auto-Fill Numeric Dialogs", !(state ?? false));
                    state = Svc.PluginInterface.GetIpcSubscriber<string, bool?>($"PandorasBox.GetFeatureEnabled").InvokeFunc("Auto-Fill Numeric Dialogs");
                    Svc.Log.Debug($"State of Auto-Fill Numeric Dialogs after setting: {state}");
                }

                if (ImGui.Button("Set Ingredients"))
                {
                    CraftingListFunctions.SetIngredients();
                }

                if (TryGetAddonByName<AtkUnitBase>("RetainerHistory", out var addon))
                {
                    var list = addon->UldManager.SearchNodeById(10)->GetAsAtkComponentList();
                    ImGui.Text($"{list->ListLength}");
                }
            }
            catch (Exception e)
            {
                e.Log();
            }

            ImGui.Text($"{Crafting.CurState}");
            ImGui.Text($"{PreCrafting.Tasks.Count()}");
            ImGui.Text($"{P.TM.IsBusy}");
            ImGui.Text($"{CraftingListFunctions.CLTM.IsBusy}");

            foreach (var obj in Svc.Objects.Where(x => x.IsTargetable).OrderBy(x => x.Name))
            {
                if (ImGui.Button($"{obj.Name} - {obj.Position}"))
                {
                    Chat.Instance.SendMessage($"/vnavmesh flyto {obj.Position.X} {obj.Position.Y - obj.HitboxRadius} {obj.Position.Z}");
                }
            }
        }

        private static void DrawRecipeEntry(string tag, RecipeNoteRecipeEntry* e)
        {
            var recipe = Svc.Data.GetExcelSheet<Recipe>()?.GetRow(e->RecipeId);
            using var n = ImRaii.TreeNode($"{tag}: {e->RecipeId} '{recipe?.ItemResult.Value?.Name}'###{tag}");
            if (!n)
                return;

            int i = 0;
            foreach (ref var ing in e->IngredientsSpan)
            {
                if (ing.NumTotal != 0)
                {
                    var item = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>()?.GetRow(ing.ItemId);
                    using var n1 = ImRaii.TreeNode($"Ingredient {i}: {ing.ItemId} '{item?.Name}' (ilvl={item?.LevelItem.Row}, hq={item?.CanBeHq}), max={ing.NumTotal}, nq={ing.NumAssignedNQ}/{ing.NumAvailableNQ}, hq={ing.NumAssignedHQ}/{ing.NumAvailableHQ}", ImGuiTreeNodeFlags.Leaf);
                }
                i++;
            }

            if (recipe != null)
            {
                var startingQuality = Calculations.GetStartingQuality(recipe, e->GetAssignedHQIngredients());
                using var n2 = ImRaii.TreeNode($"Starting quality: {startingQuality}/{Calculations.RecipeMaxQuality(recipe)}", ImGuiTreeNodeFlags.Leaf);
            }
        }

        private static void DrawEquippedGear()
        {
            using var nodeEquipped = ImRaii.TreeNode("Equipped gear");
            if (!nodeEquipped)
                return;

            var stats = CharacterStats.GetBaseStatsEquipped();
            ImGui.TextUnformatted($"Total stats: {stats.Craftsmanship}/{stats.Control}/{stats.CP}/{stats.Splendorous}/{stats.Specialist}");

            var inventory = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            if (inventory == null)
                return;

            for (int i = 0; i < inventory->Size; ++i)
            {
                var item = inventory->Items + i;
                var details = new ItemStats(item);
                if (details.Data == null)
                    continue;

                using var n = ImRaii.TreeNode($"{i}: {item->ItemID} '{details.Data.Name}' ({item->Flags}): crs={details.Stats[0].Base}+{details.Stats[0].Melded}/{details.Stats[0].Max}, ctrl={details.Stats[1].Base}+{details.Stats[1].Melded}/{details.Stats[1].Max}, cp={details.Stats[2].Base}+{details.Stats[2].Melded}/{details.Stats[2].Max}");
                if (n)
                {
                    for (int j = 0; j < 5; ++j)
                    {
                        using var m = ImRaii.TreeNode($"Materia {j}: {item->Materia[j]} {item->MateriaGrade[j]}", ImGuiTreeNodeFlags.Leaf);
                    }
                }
            }
        }

        private static void DrawGearset(ref RaptureGearsetModule.GearsetEntry gs)
        {
            if (!gs.Flags.HasFlag(RaptureGearsetModule.GearsetFlag.Exists))
                return;

            fixed (byte* name = gs.Name)
            {
                using var nodeGearset = ImRaii.TreeNode($"Gearset {gs.ID} '{Dalamud.Memory.MemoryHelper.ReadString((nint)name, 48)}' {(Job)gs.ClassJob} ({gs.Flags})");
                if (!nodeGearset)
                    return;

                var stats = CharacterStats.GetBaseStatsGearset(ref gs);
                ImGui.TextUnformatted($"Total stats: {stats.Craftsmanship}/{stats.Control}/{stats.CP}/{stats.Splendorous}/{stats.Specialist}");

                for (int i = 0; i < gs.ItemsSpan.Length; ++i)
                {
                    ref var item = ref gs.ItemsSpan[i];
                    var details = new ItemStats((RaptureGearsetModule.GearsetItem*)Unsafe.AsPointer(ref item));
                    if (details.Data == null)
                        continue;

                    using var n = ImRaii.TreeNode($"{i}: {item.ItemID} '{details.Data.Name}' ({item.Flags}): crs={details.Stats[0].Base}+{details.Stats[0].Melded}/{details.Stats[0].Max}, ctrl={details.Stats[1].Base}+{details.Stats[1].Melded}/{details.Stats[1].Max}, cp={details.Stats[2].Base}+{details.Stats[2].Melded}/{details.Stats[2].Max}");
                    if (n)
                    {
                        for (int j = 0; j < 5; ++j)
                        {
                            using var m = ImRaii.TreeNode($"Materia {j}: {item.Materia[j]} {item.MateriaGrade[j]}", ImGuiTreeNodeFlags.Leaf);
                        }
                    }
                }
            }
        }

        public class Item
        {
            public uint Key { get; set; }
            public string Name { get; set; } = "";
            public ushort CraftingTime { get; set; }
            public uint UIIndex { get; set; }
        }
    }
}
