using System;
using System.Collections.Generic;
using System.Windows;

namespace TreeLibrary.Model
{
    public interface ILoadDataAndTemplate
    {
        Dictionary<Type, Type> GetLogicDictionary();
        void SetStyle(List<Type> modelItemList, ResourceDictionary controlResources);
        Dictionary<string, Dictionary<RoutedEvent, System.Delegate>> GetMenuRouteAndHandlerDictionary();
    }
}