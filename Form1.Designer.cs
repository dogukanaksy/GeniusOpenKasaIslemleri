namespace KasaVersiyonGuncelleme
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnBasla = new Button();
            logRichtext = new RichTextBox();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            label2 = new Label();
            label5 = new Label();
            label7 = new Label();
            txtDsil = new TextBox();
            txtCpVeriEkle = new TextBox();
            txtPosini = new TextBox();
            btnDsil = new Button();
            btnLibAktar = new Button();
            btnCpVeriEkle = new Button();
            btnTpAktar = new Button();
            btnPosini = new Button();
            toolTip1 = new ToolTip(components);
            button1 = new Button();
            txtOldName = new TextBox();
            label3 = new Label();
            txtNewName = new TextBox();
            label4 = new Label();
            label6 = new Label();
            SuspendLayout();
            // 
            // btnBasla
            // 
            btnBasla.Location = new Point(12, 647);
            btnBasla.Name = "btnBasla";
            btnBasla.Size = new Size(196, 53);
            btnBasla.TabIndex = 0;
            btnBasla.Text = "Kasa Versiyonu Güncelle";
            btnBasla.UseVisualStyleBackColor = true;
            btnBasla.Click += button1_Click;
            btnBasla.MouseHover += btnBasla_MouseHover;
            // 
            // logRichtext
            // 
            logRichtext.Location = new Point(12, 371);
            logRichtext.Name = "logRichtext";
            logRichtext.Size = new Size(936, 269);
            logRichtext.TabIndex = 2;
            logRichtext.Text = "";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(635, 656);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(313, 29);
            progressBar1.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label1.Location = new Point(557, 654);
            label1.Name = "label1";
            label1.Size = new Size(72, 28);
            label1.TabIndex = 5;
            label1.Text = "Durum";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 118);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 6;
            label2.Text = "Dosya Sil";
            label2.MouseHover += label2_MouseHover;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(27, 177);
            label5.Name = "label5";
            label5.Size = new Size(112, 20);
            label5.TabIndex = 9;
            label5.Text = "Cp.Bat Veri Ekle";
            label5.MouseHover += label5_MouseHover;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(27, 226);
            label7.Name = "label7";
            label7.Size = new Size(212, 20);
            label7.TabIndex = 11;
            label7.Text = "Eve Teslim Servis Linki Düzenle";
            label7.MouseHover += label7_MouseHover;
            // 
            // txtDsil
            // 
            txtDsil.Location = new Point(260, 117);
            txtDsil.Name = "txtDsil";
            txtDsil.Size = new Size(416, 27);
            txtDsil.TabIndex = 12;
            // 
            // txtCpVeriEkle
            // 
            txtCpVeriEkle.Location = new Point(260, 170);
            txtCpVeriEkle.Name = "txtCpVeriEkle";
            txtCpVeriEkle.Size = new Size(416, 27);
            txtCpVeriEkle.TabIndex = 14;
            // 
            // txtPosini
            // 
            txtPosini.Location = new Point(260, 226);
            txtPosini.Name = "txtPosini";
            txtPosini.Size = new Size(416, 27);
            txtPosini.TabIndex = 16;
            // 
            // btnDsil
            // 
            btnDsil.Location = new Point(721, 113);
            btnDsil.Name = "btnDsil";
            btnDsil.Size = new Size(227, 35);
            btnDsil.TabIndex = 18;
            btnDsil.Text = "Dosya Sil";
            btnDsil.UseVisualStyleBackColor = true;
            btnDsil.Click += btnDsil_Click;
            btnDsil.MouseHover += btnDsil_MouseHover;
            // 
            // btnLibAktar
            // 
            btnLibAktar.Location = new Point(755, 278);
            btnLibAktar.Name = "btnLibAktar";
            btnLibAktar.Size = new Size(181, 35);
            btnLibAktar.TabIndex = 20;
            btnLibAktar.Text = "Libe Dosya Aktar";
            btnLibAktar.UseVisualStyleBackColor = true;
            btnLibAktar.Click += btnLibAktar_Click;
            btnLibAktar.MouseHover += btnLibAktar_MouseHover;
            // 
            // btnCpVeriEkle
            // 
            btnCpVeriEkle.Location = new Point(721, 170);
            btnCpVeriEkle.Name = "btnCpVeriEkle";
            btnCpVeriEkle.Size = new Size(227, 35);
            btnCpVeriEkle.TabIndex = 21;
            btnCpVeriEkle.Text = "Cp.Bat Veri Ekle";
            btnCpVeriEkle.UseVisualStyleBackColor = true;
            btnCpVeriEkle.Click += btnCpVeriEkle_Click;
            btnCpVeriEkle.MouseHover += btnCpVeriEkle_MouseHover;
            // 
            // btnTpAktar
            // 
            btnTpAktar.Location = new Point(27, 278);
            btnTpAktar.Name = "btnTpAktar";
            btnTpAktar.Size = new Size(181, 35);
            btnTpAktar.TabIndex = 22;
            btnTpAktar.Text = "TransferProperties Aktar";
            btnTpAktar.UseVisualStyleBackColor = true;
            btnTpAktar.Click += btnTpAktar_Click;
            btnTpAktar.MouseHover += btnTpAktar_MouseHover;
            // 
            // btnPosini
            // 
            btnPosini.Location = new Point(721, 226);
            btnPosini.Name = "btnPosini";
            btnPosini.Size = new Size(227, 35);
            btnPosini.TabIndex = 23;
            btnPosini.Text = "Eve Teslim Servis Linki Düzenle";
            btnPosini.UseVisualStyleBackColor = true;
            btnPosini.Click += btnPosini_Click;
            btnPosini.MouseHover += btnPosini_MouseHover;
            // 
            // toolTip1
            // 
            toolTip1.Popup += toolTip1_Popup;
            // 
            // button1
            // 
            button1.Location = new Point(721, 54);
            button1.Name = "button1";
            button1.Size = new Size(227, 35);
            button1.TabIndex = 26;
            button1.Text = "Dosya Adını Değiştir";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            button1.MouseHover += button1_MouseHover;
            // 
            // txtOldName
            // 
            txtOldName.Location = new Point(260, 58);
            txtOldName.Name = "txtOldName";
            txtOldName.Size = new Size(176, 27);
            txtOldName.TabIndex = 25;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(27, 59);
            label3.Name = "label3";
            label3.Size = new Size(145, 20);
            label3.TabIndex = 24;
            label3.Text = "Dosya Adını Değiştir";
            label3.MouseHover += label3_MouseHover;
            // 
            // txtNewName
            // 
            txtNewName.Location = new Point(501, 59);
            txtNewName.Name = "txtNewName";
            txtNewName.Size = new Size(175, 27);
            txtNewName.TabIndex = 27;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(278, 24);
            label4.Name = "label4";
            label4.Size = new Size(126, 20);
            label4.TabIndex = 28;
            label4.Text = "Dosyanın Eski Adı";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(526, 24);
            label6.Name = "label6";
            label6.Size = new Size(128, 20);
            label6.TabIndex = 29;
            label6.Text = "Dosyanın Yeni Adı";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(967, 721);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(txtNewName);
            Controls.Add(button1);
            Controls.Add(txtOldName);
            Controls.Add(label3);
            Controls.Add(btnPosini);
            Controls.Add(btnTpAktar);
            Controls.Add(btnCpVeriEkle);
            Controls.Add(btnLibAktar);
            Controls.Add(btnDsil);
            Controls.Add(txtPosini);
            Controls.Add(txtCpVeriEkle);
            Controls.Add(txtDsil);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(progressBar1);
            Controls.Add(logRichtext);
            Controls.Add(btnBasla);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Kasa Manuel İşlemler";
            Load += Form1_Load_1;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBasla;
        private RichTextBox logRichtext;
        private ProgressBar progressBar1;
        private Label label1;
        private Label label2;
        private Label label5;
        private Label label7;
        private TextBox txtDsil;
        private TextBox txtCpVeriEkle;
        private TextBox txtPosini;
        private Button btnDsil;
        private Button btnLibAktar;
        private Button btnCpVeriEkle;
        private Button btnTpAktar;
        private Button btnPosini;
        private ToolTip toolTip1;
        private Button button1;
        private TextBox txtOldName;
        private Label label3;
        private TextBox txtNewName;
        private Label label4;
        private Label label6;
    }
}