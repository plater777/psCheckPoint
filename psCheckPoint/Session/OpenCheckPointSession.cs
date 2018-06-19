﻿using Koopman.CheckPoint;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace psCheckPoint.Session
{
    /// <api cmd="login">Open-CheckPointSession</api>
    /// <summary>
    /// <para type="synopsis">Log in to the server with user name and password.</para>
    /// <para type="description"></para>
    /// </summary>
    /// <example>
    /// <code>
    /// Open-CheckPointSession -ManagementServer 192.168.1.1
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// $Session = Open-CheckPointSession -ManagementServer 192.168.1.1 -PassThru
    /// </code>
    /// </example>
    [Cmdlet(VerbsCommon.Open, "CheckPointSession")]
    [OutputType(typeof(Koopman.CheckPoint.Session))]
    public class OpenCheckPointSession : PSCmdletAsync
    {
        #region Properties

        /// <summary>
        /// <para type="description">Server certificate hash you are expecting.</para>
        /// </summary>
        [Parameter]
        public string CertificateHash { get; set; }

        /// <summary>
        /// <para type="description">What level of certificate validation to perform.</para>
        /// </summary>
        [Parameter]
        public CertificateValidation CertificateValidation { get; set; } = CertificateValidation.Auto;

        /// <summary>
        /// <para type="description">The new session would continue where the last session was stopped.</para>
        /// <para type="description">
        /// This option is available when the administrator has only one session that can be continued.
        /// </para>
        /// <para type="description">If there is more than one session, see 'switch-session' API.</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ContinueLastSession { get; set; }

        /// <summary>
        /// <para type="description">
        /// PSCredential containing User name and Password. If not provided you will be prompted.
        /// </para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = true)]
        public PSCredential Credentials { get; set; }

        /// <summary>
        /// <para type="description">
        /// Use domain to login to specific domain. Domain can be identified by name or UID.
        /// </para>
        /// </summary>
        [Parameter]
        public string Domain { get; set; }

        /// <summary>
        /// <para type="description">
        /// Login to the last published session. Such login is done with the Read Only permissions.
        /// </para>
        /// </summary>
        [Parameter]
        public SwitchParameter EnterLastPublishedSession { get; set; }

        /// <summary>
        /// <para type="description">HTTP Request timeout in seconds. Default 100 seconds.</para>
        /// <para type="description">Use -1 for infinate</para>
        /// </summary>
        [Parameter]
        public int HttpTimeout { get; set; } = 100;

        /// <summary>
        /// <para type="description">Port Web API running on. Default: 443</para>
        /// </summary>
        [Parameter]
        public int ManagementPort { get; set; } = 443;

        /// <summary>
        /// <para type="description">IP or Hostname of the Check point Management Server</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true)]
        public string ManagementServer { get; set; }

        /// <summary>
        /// <para type="description">Return the session and do not store it for automatic use.</para>
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// <para type="description">
        /// Login with Read Only permissions. This parameter is not considered in case
        /// continue-last-session is true.
        /// </para>
        /// </summary>
        [Parameter]
        public SwitchParameter ReadOnly { get; set; }

        /// <summary>
        /// <para type="description">Session comments.</para>
        /// </summary>
        [Parameter]
        public string SessionComments { get; set; }

        /// <summary>
        /// <para type="description">Session description.</para>
        /// </summary>
        [Parameter]
        public string SessionDescription { get; set; }

        /// <summary>
        /// <para type="description">Session unique name.</para>
        /// </summary>
        [Parameter]
        public string SessionName { get; set; }

        /// <summary>
        /// <para type="description">Session expiration timeout in seconds. Default 600 seconds.</para>
        /// </summary>
        [Parameter]
        public int SessionTimeout { get; set; } = 600;

        private string Password { get; set; }
        private string User { get; set; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        protected override async Task ProcessRecordAsync()
        {
            try
            {
                var session = await Koopman.CheckPoint.Session.Login(
                    managementServer: ManagementServer,
                    domain: Domain,
                    userName: Credentials.GetNetworkCredential().UserName,
                    password: Credentials.GetNetworkCredential().Password,
                    port: ManagementPort,
                    readOnly: ReadOnly.IsPresent,
                    certificateValidation: CertificateValidation,
                    certificateHash: CertificateHash,
                    sessionName: SessionName,
                    comments: SessionComments,
                    description: SessionDescription,
                    timeout: SessionTimeout,
                    continueLastSession: ContinueLastSession.IsPresent,
                    enterLastPublishedSession: EnterLastPublishedSession.IsPresent,
                    cancellationToken: CancelProcessToken,
                    httpTimeout: (HttpTimeout < 0) ? Timeout.InfiniteTimeSpan : new TimeSpan(0, 0, HttpTimeout)
                    );
                if (PassThru.IsPresent)
                    WriteObject(session);
                else
                    SessionState.PSVariable.Set("CheckPointSession", session);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                WriteError(new ErrorRecord(e, e.Message, ErrorCategory.ConnectionError, this));
            }
        }

        #endregion Methods
    }
}