﻿using HateoasGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace HateoasGenerator.Interfaces;
internal static class IHateoasFactory
{
    private const string FileName = "IHateoasFactory";
    private const string Class = @"
using HateoasLib.Models.ResponseModels;
using HateoasLib.Models;

namespace HateoasLib.Interfaces;

public interface IHateoasFactory
{
    Resource<T> CreateResponse<T, R>(
                                    string controller,
                                    T item,
                                    List<ControllerAction<T, R>> itemActions);

   CollectionResource<T> CreateCollectionResponse<T, R>(
                                                        string controller,
                                                        IEnumerable<T> items,
                                                        List<ControllerAction> listActions,
                                                        List<ControllerAction<T, R>> itemActions);

    Resource<T> CreateResponse<T>(
                            string controller,
                            T item,
                            List<ControllerAction> itemActions);

    PaginatedResource<T> CreatePaginatedResponse<T>(
                                                    string controller,
                                                    IEnumerable<T> items,
                                                    List<ControllerAction> listActions,
                                                    List<ControllerAction<T, object>> itemActions);
}";

    internal static IncrementalGeneratorInitializationContext AddIHateoasFactoryToSource(this IncrementalGeneratorInitializationContext context)
    {
        return context.AddFileToSource(Class, FileName);
    }
}
