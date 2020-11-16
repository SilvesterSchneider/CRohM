using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public enum ApprovedStatus
    {
        Approved = 0,
        AlreadyApprovedOrDeleted = 1,
        InvalidId = 2,
        ErrorInSaving = 3
    }

    public class ApproveDto
    {
        public ApprovedStatus ApprovedState { get; set; }
    }
}
