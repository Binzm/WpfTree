using System;
using System.Collections.Generic;
using System.Windows;

namespace TreeLibrary.Model
{
    public interface ILoadDataAndTemplate
    {
        bool GetIsAllowDrop();
        Dictionary<Type, Type> GetLogicDictionary();
        Dictionary<RoutedEvent, System.Delegate> GetTreeRoutedHandler();
        Dictionary<RoutedEvent, System.Delegate> GetTreeLibraryRoutedHandler();
        void SetStyle(List<Type> modelItemList, ResourceDictionary controlResources);
        Dictionary<string, Dictionary<RoutedEvent, System.Delegate>> GetMenuRouteAndHandlerDictionary();
    }
}