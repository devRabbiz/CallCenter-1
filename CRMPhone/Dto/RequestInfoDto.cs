﻿using System;

namespace CRMPhone.Dto
{
    public class RequestInfoDto
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public AddressDto Address { get; set; }
        public string Description { get; set; }
        public string Entrance { get; set; }
        public string Floor { get; set; }
        public RequestTypeDto Type { get; set; }
        public RequestStateDto State { get; set; }
        public RequestUserDto CreateUser { get; set; }
        public ContactDto[] Contacts { get; set; }
        public DateTime? ExecuteDate { get; set; }
        public int? PeriodId { get; set; }
        public int? ExecutorId { get; set; }
        public bool IsImmediate { get; set; }
        public bool IsChargeable { get; set; }
        public RequestRatingDto Rating { get; set; }
    }
}