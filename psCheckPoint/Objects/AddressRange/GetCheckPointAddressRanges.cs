﻿using System.Management.Automation;

namespace psCheckPoint.Objects.AddressRange
{
    /// <api cmd="show-address-ranges">Get-CheckPointAddressRanges</api>
    /// <summary>
    /// <para type="synopsis">Retrieve all objects.</para>
    /// <para type="description"></para>
    /// </summary>
    /// <example></example>
    [Cmdlet(VerbsCommon.Get, "CheckPointAddressRanges")]
    [OutputType(typeof(Koopman.CheckPoint.Common.ObjectsPagingResults<Koopman.CheckPoint.AddressRange>))]
    public class GetCheckPointAddressRanges : GetCheckPointObjects
    {
        #region Methods

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var results = Session.FindAllAddressRanges(
                limit: Limit,
                offset: Offset,
                detailLevel: DetailsLevel
                );

            if (ParameterSetName == "Limit")
            {
                WriteObject(results, false);
            }
            else
            {
                while (results != null)
                {
                    foreach (var r in results)
                    {
                        WriteObject(r);
                    }
                    results = results.NextPage();
                }
            }
        }

        #endregion Methods
    }
}