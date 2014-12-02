namespace ECViewService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ECViewServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ECViewServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ECViewServiceProcessInstaller
            // 
            this.ECViewServiceProcessInstaller.Password = null;
            this.ECViewServiceProcessInstaller.Username = null;
            this.ECViewServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // ECViewServiceInstaller
            // 
            this.ECViewServiceInstaller.Description = "ECView Service";
            this.ECViewServiceInstaller.DisplayName = "ECViewService";
            this.ECViewServiceInstaller.ServiceName = "ECViewService";
            this.ECViewServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ECViewServiceProcessInstaller,
            this.ECViewServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ECViewServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ECViewServiceInstaller;
    }
}