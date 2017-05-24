﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CRMPhone.Dto;
using CRMPhone.ViewModel;

namespace CRMPhone
{
    /// <summary>
    /// Логика взаимодействия для RequestDialog.xaml
    /// </summary>
    public partial class RequestDialog : Window
    {
        private RequestDialogViewModel _context;

        public RequestDialog(RequestDialogViewModel context)
        {
            _context = context;

            Owner = Application.Current.MainWindow;
            DataContext = _context;

            _context.SetView(this);
            InitializeComponent();
        }

        private void SelectCurrentContact(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            _context.SelectedContact = (ContactDto)item.Content;
        }
        private void SelectCurrentRequest(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            _context.SelectedRequest = (RequestItemViewModel)item.Content;
        }

        private void tabItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //e.Key = Key.Tab;
                var request = new TraversalRequest(FocusNavigationDirection.Right) { Wrapped = true };
                if (sender is ComboBox)
                {
                    var parentDepObj = VisualTreeHelper.GetParent(sender as DependencyObject);
                    var comboBoxes = (parentDepObj as WrapPanel).Children.OfType<ComboBox>().ToList();
                    var currentIndex = comboBoxes.IndexOf(sender as ComboBox);
                    if (currentIndex < comboBoxes.Count - 1)
                        comboBoxes[currentIndex + 1].Focus();
                    //var t = (sender as ComboBox).PredictFocus(FocusNavigationDirection.Next);
                    //(sender as ComboBox).MoveFocus(request);
                }
            }
        }
    }
}
