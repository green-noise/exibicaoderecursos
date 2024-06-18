namespace exibição_de_recursos_CS // Defines a namespace for the project.
{
    partial class MainForm // Partial class definition for MainForm.
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null; // Container for holding the components of the form.

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) // Dispose method to clean up resources.
        {
            if (disposing && (components != null)) // Checks if disposing is true and components is not null.
            {
                components.Dispose(); // Disposes the components.
            }
            base.Dispose(disposing); // Calls the base class Dispose method.
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() // Method for initializing the form's components.
        {
            this.SuspendLayout(); // Suspends the layout logic for the form.
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F); // Sets the scaling dimensions.
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // Sets the scaling mode.
            this.ClientSize = new System.Drawing.Size(400, 250); // Sets the size of the form.
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // Sets the form border style to none.
            this.Name = "MainForm"; // Sets the name of the form.
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual; // Sets the start position of the form to manual.
            this.TopMost = true; // Sets the form to be always on top.
            this.BackColor = System.Drawing.Color.Black; // Sets the background color of the form to black.
            this.Opacity = 0.5; // Sets the opacity of the form to 50%.
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint); // Attaches the Paint event handler.
            this.ResumeLayout(false); // Resumes the layout logic for the form.
        }

        #endregion
    }
}