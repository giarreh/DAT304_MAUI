﻿using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp8.Services.BackgroundServices.Realm;

namespace MauiApp8.Services.Food
{
    public class FoodService : IFoodService
    {
        private readonly IUtils _utils;
        private readonly ICRUD _crud;

        public async Task MockFood()
        {
            using var realm = _utils.RealmCreate();
            await _crud.MockFood(realm);
        }

        public FoodService(IUtils utils, ICRUD crud)
        {
            _utils = utils;
            _crud = crud;
        }

        public async Task<List<Model.Food>> GetFoods()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFoods(realm).ConvertAll(ClassConvert.ToModel); ;
        }

        public async Task<Model.Food> GetFood(string name)
        {
            using var realm = _utils.RealmCreate();
            return ClassConvert.ToModel(_crud.ReadFood(realm, name));
        }

        public async Task AddFood(string name, float calories, float carbohydrates, float protein, float fat)
        {
            using var realm = _utils.RealmCreate();
            await _crud.AddFood(realm, name, calories, carbohydrates, protein, fat);
        }

        public async Task UpdateFood(string name, float calories, float carbohydrates, float protein, float fat)
        {
            using var realm = _utils.RealmCreate();
            await _crud.UpdateFood(realm, name, calories, carbohydrates, protein, fat);
        }

        public async Task DeleteFood(string foodName)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteFood(realm, foodName);
        }

        public async Task<List<Model.FoodEntry>> GetFoodEntries()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadFoodEntries(realm).ConvertAll(ClassConvert.ToModel);
        }

        public async Task<Model.FoodEntry> GetFoodEntry(int foodEntryId)
        {
            using var realm = _utils.RealmCreate();
            return ClassConvert.ToModel(_crud.ReadFoodEntry(realm, foodEntryId));
        }

        public async Task<int> CreateFoodEntry(string foodName, float amount)
        {
            using var realm = _utils.RealmCreate();
            return await _crud.CreateFoodEntry(realm, foodName, amount);
        }

        public async Task DeleteFoodEntry(int foodEntryId)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteFoodEntry(realm, foodEntryId);
        }

        public async Task<List<Model.Meal>> GetMeals()
        {
            using var realm = _utils.RealmCreate();
            return _crud.ReadMeals(realm).ConvertAll(ClassConvert.ToModel);
        }

        public async Task<Model.Meal> GetMeal(int mealId)
        {
            using var realm = _utils.RealmCreate();
            return ClassConvert.ToModel(_crud.ReadMeal(realm, mealId));
        }

        public async Task<int> CreateMeal(List<int> foodEntryIds)
        {
            using var realm = _utils.RealmCreate();
            return await _crud.CreateMeal(realm, foodEntryIds);
        }

        public async Task DeleteMeal(int mealId)
        {
            using var realm = _utils.RealmCreate();
            await _crud.DeleteMeal(realm, mealId);
        }
    }
}
