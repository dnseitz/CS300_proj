namespace BATSystem
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PictureMap = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.InputLocationTextbox = new System.Windows.Forms.TextBox();
            this.InputNameTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.Vehicle = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.InputDispatcherTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.PictureMap.Location = new System.Drawing.Point(225, 12);
            this.PictureMap.Name = "pictureBox1";
            this.PictureMap.Size = new System.Drawing.Size(711, 455);
            this.PictureMap.TabIndex = 0;
            this.PictureMap.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(69, 392);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Sign in";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SignInButton_Click);
            // 
            // textBox1
            // 
            this.InputLocationTextbox.Location = new System.Drawing.Point(61, 222);
            this.InputLocationTextbox.Name = "textBox1";
            this.InputLocationTextbox.Size = new System.Drawing.Size(158, 20);
            this.InputLocationTextbox.TabIndex = 2;
            this.InputLocationTextbox.TextChanged += new System.EventHandler(this.InputLocationField_TextChanged);
            // 
            // textBox3
            // 
            this.InputNameTextbox.Location = new System.Drawing.Point(61, 172);
            this.InputNameTextbox.Name = "textBox3";
            this.InputNameTextbox.Size = new System.Drawing.Size(158, 20);
            this.InputNameTextbox.TabIndex = 4;
            this.InputNameTextbox.TextChanged += new System.EventHandler(this.InputNameField_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Location";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Name";
            // 
            // comboBox1
            // 
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Items.AddRange(new object[] {
            "Police Car",
            "Ambulance",
            "Fire Truck"});
            this.TypeComboBox.Location = new System.Drawing.Point(61, 270);
            this.TypeComboBox.Name = "comboBox1";
            this.TypeComboBox.Size = new System.Drawing.Size(158, 21);
            this.TypeComboBox.TabIndex = 7;
            this.TypeComboBox.SelectedIndexChanged += new System.EventHandler(this.TypeOfVehicleComboBox_SelectedIndexChanged);
            // 
            // Vehicle
            // 
            this.Vehicle.AutoSize = true;
            this.Vehicle.Location = new System.Drawing.Point(12, 273);
            this.Vehicle.Name = "Vehicle";
            this.Vehicle.Size = new System.Drawing.Size(42, 13);
            this.Vehicle.TabIndex = 8;
            this.Vehicle.Text = "Vehicle";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(150, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Connect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // textBox2
            // 
            this.InputDispatcherTextbox.Location = new System.Drawing.Point(44, 58);
            this.InputDispatcherTextbox.Name = "textBox2";
            this.InputDispatcherTextbox.Size = new System.Drawing.Size(100, 20);
            this.InputDispatcherTextbox.TabIndex = 10;
            this.InputDispatcherTextbox.TextChanged += new System.EventHandler(this.DispatcherIPField_TextChanged_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "local IP";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Connected";
            this.label4.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 479);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.InputDispatcherTextbox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Vehicle);
            this.Controls.Add(this.TypeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputNameTextbox);
            this.Controls.Add(this.InputLocationTextbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PictureMap);
            this.Name = "Form1";
            this.Text = "Emergency Vehicle System";
            ((System.ComponentModel.ISupportInitialize)(this.PictureMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureMap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox InputLocationTextbox;
        private System.Windows.Forms.TextBox InputNameTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox TypeComboBox;
        private System.Windows.Forms.Label Vehicle;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox InputDispatcherTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

    }
}

