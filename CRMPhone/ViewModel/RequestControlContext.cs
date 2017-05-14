using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using CRMPhone.Annotations;
using CRMPhone.Dto;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace CRMPhone.ViewModel
{
    public class RequestControlContext : INotifyPropertyChanged
    {
        private ObservableCollection<RequestForListDto> _requestList;
        private RequestService _requestService;

        private ICommand _addRequestCommand;
        public ICommand AddRequestCommand { get { return _addRequestCommand ?? (_addRequestCommand = new CommandHandler(AddRequest, true)); } }
        private ICommand _refreshRequestCommand;
        public ICommand RefreshRequestCommand { get { return _refreshRequestCommand ?? (_refreshRequestCommand = new CommandHandler(RefreshRequest, true)); } }
        private ICommand _exportRequestCommand;
        public ICommand ExportRequestCommand { get { return _exportRequestCommand ?? (_exportRequestCommand = new CommandHandler(ExportRequest, true)); } }

        private void ExportRequest()
        {
            if (RequestList.Count == 0)
            {
                MessageBox.Show("������ �������������� ������ ������!","������");
                return;
            }
            try
            {

                var saveDialog = new SaveFileDialog();
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".xml";
                saveDialog.Filter = "XML ����|*.xml";
                if (saveDialog.ShowDialog() == true)
                {
                    var fileName = saveDialog.FileName;


                    XElement root = new XElement("Records");
                    foreach (var request in RequestList)
                    {
                        root.AddFirst(
                            new XElement("Record",
                                new []
                                {
                                    new XElement("������", request.Id),
                                    new XElement("������������", request.CreateTime.ToString("dd.MM.yyyy HH:mm")),
                                    new XElement("���������", request.CreateUser.ShortName),
                                    new XElement("�����", request.StreetName),
                                    new XElement("���", request.Building),
                                    new XElement("������", request.Corpus),
                                    new XElement("��������", request.Flat),
                                    new XElement("��������", request.ContactPhones),
                                    new XElement("������", request.ParentService),
                                    new XElement("�������", request.Service),
                                    new XElement("����������", request.Description),
                                    new XElement("����", request.ExecuteTime?.Date.ToString("dd.MM.yyyy") ?? ""),
                                    new XElement("�����", request.ExecutePeriod),
                                    new XElement("�����������", request.Worker?.ShortName),
                                }));


                    }
                    var saver = new FileStream(fileName, FileMode.Create);
                    root.Save(saver);
                    saver.Close();
                    MessageBox.Show("������ ��������� � ����\r\n" + fileName);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("��������� ������:\r\n" + exc.Message);
            }

        }

        private ICommand _openRequestCommand;
        private DateTime _fromDate;
        private DateTime _toDate;
        public ICommand OpenRequestCommand { get { return _openRequestCommand ?? (_openRequestCommand = new RelayCommand(OpenRequest));} }

        private void OpenRequest(object sender)
        {
            var selectedItem = sender as RequestForListDto;
            if (selectedItem == null)
                return;
            if (_requestService == null)
                _requestService = new RequestService(AppSettings.DbConnection);

            var request = _requestService.GetRequest(selectedItem.Id);
            if (request == null)
            {
                MessageBox.Show("��������� �������������� ������!");
                return;
            }

            var viewModel = new RequestDialogViewModel();
            var view = new RequestDialog(viewModel);
            viewModel.SetView(view);
            viewModel.SelectedCity = viewModel.CityList.SingleOrDefault(i=>i.Id == request.Address.CityId);
            viewModel.SelectedStreet = viewModel.StreetList.SingleOrDefault(i => i.Id == request.Address.StreetId);
            viewModel.SelectedHouse = viewModel.HouseList.SingleOrDefault(i=>i.Id == request.Address.HouseId);
            viewModel.SelectedFlat =  viewModel.FlatList.SingleOrDefault(i=>i.Id == request.Address.Id);
            var requestModel = viewModel.RequestList.FirstOrDefault();
            requestModel.SelectedParentService = requestModel.ParentServiceList.SingleOrDefault(i => i.Id == request.Type.ParentId);
            requestModel.SelectedService = requestModel.ServiceList.SingleOrDefault(i => i.Id == request.Type.Id);
            requestModel.Description = request.Description;
            requestModel.IsChargeable = request.IsChargeable;
            requestModel.IsImmediate = request.IsImmediate;
            requestModel.RequestCreator = request.CreateUser.ShortName;
            requestModel.RequestDate = request.CreateTime;
            requestModel.RequestState = request.State.Description;
            requestModel.SelectedWorker = requestModel.WorkerList.SingleOrDefault(w => w.Id == request.ExecutorId);
            requestModel.RequestId = request.Id;
            if (request.ExecuteDate.HasValue && request.ExecuteDate.Value.Date > DateTime.MinValue)
            {
                requestModel.SelectedDateTime = request.ExecuteDate.Value.Date;
                requestModel.SelectedPeriod = requestModel.PeriodList.SingleOrDefault(i => i.Id == request.PeriodId);
            }
            viewModel.RequestId = request.Id;
            viewModel.ContactList = new ObservableCollection<ContactDto>(request.Contacts);
            var t = view.ShowDialog();

        }

        private void RefreshRequest()
        {
            if(_requestService == null)
                _requestService = new RequestService(AppSettings.DbConnection);
            RequestList.Clear();
            var requests = _requestService.GetRequestList(FromDate,ToDate);
            foreach (var request in requests)
            {
                RequestList.Add(request);
            }
            OnPropertyChanged(nameof(RequestList));
        }

        public RequestControlContext()
        {
            RequestList = new ObservableCollection<RequestForListDto>();
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
        }

        public ObservableCollection<RequestForListDto> RequestList
        {
            get { return _requestList; }
            set { _requestList = value; OnPropertyChanged(nameof(RequestList));}
        }

        public DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; OnPropertyChanged(nameof(FromDate));}
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; OnPropertyChanged(nameof(ToDate));}
        }


        private void AddRequest()
        {
            var viewModel = new RequestDialogViewModel();
            var view = new RequestDialog(viewModel);
            viewModel.SetView(view);
            var t = view.ShowDialog();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}