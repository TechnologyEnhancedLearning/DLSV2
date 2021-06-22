namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;
    [Migration(202105261019)]
    public class FrameworksWorkforceManagerChanges : Migration
    {
        public override void Up()
        {
            Create.Table("NRPProfessionalGroups")
               .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("ProfessionalGroup").AsString(255).NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Administrative services" }); //1
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Allied health professionals" }); //2
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Emergency services" }); //3
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Health science services" }); //4
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Nursing and midwifery" }); //5
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Personal social services" }); //6
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Support services" }); //7
            Insert.IntoTable("NRPProfessionalGroups").Row(new { ProfessionalGroup = "Professional managers" }); //8

            Create.Table("NRPSubGroups")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("NRPProfessionalGroupID").AsInt32().NotNullable().ForeignKey("NRPProfessionalGroups", "ID")
                 .WithColumn("SubGroup").AsString(255).NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Business and Projects" }); //1
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Finance" }); //2
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Human Resources" }); //3
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Information Systems" }); //4
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Information Technology" }); //5
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Legal Services" }); //6
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Public Relations" }); //7
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Patient Services" }); //8
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 1, SubGroup = "Secretarial and Clerical Generic" }); //9

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Arts Therapists" }); //10
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Chaplaincy" }); //11
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Clinical Psychologists, Counsellors and Psychotherapists" }); //12
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Dietician" }); //13
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Health Promotion" }); //14
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Orthoptists and Optometrists" }); //15
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Occupational Therapy" }); //16
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Podiatry" }); //17
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Play Specialists" }); //18
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Physiotherapy" }); //19
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Diagnostic and Therapeutic Radiography" }); //20
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Sexual Health" }); //21
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Speech and Language Therapy " }); //22
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Therapies" }); //23
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 2, SubGroup = "Theatre" }); //24

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 3, SubGroup = "Ambulance" }); //25

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Anatomical pathology" }); //26
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Biomedical Sciences" }); //27
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Clinical Sciences" }); //28
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Cytology" }); //29
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Dental Services" }); //30
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Genetic Counsellor" }); //31
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Health Care Science" }); //32
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Medical Associate Professions" }); //33
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Medical Technology" }); //34
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Orthoptists and Optometrists" }); //35
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Pharmacy" }); //36
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 4, SubGroup = "Cancer Screening" }); //37

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Combined Nursing" }); //38
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Dental Services" }); //39
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Generic (Education/Research)" }); //40
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Health Visitor" }); //41
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Midwifery" }); //42
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "NHS Direct" }); //43
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 5, SubGroup = "Theatre" }); //44

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 8, SubGroup = "Not Specified" }); //45

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 6, SubGroup = "Home Care" }); //46
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 6, SubGroup = "Residential and Day Care" }); //47
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 6, SubGroup = "Social Work" }); //48

            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 7, SubGroup = "Maintenance and Estates" }); //49
            Insert.IntoTable("NRPSubGroups").Row(new { NRPProfessionalGroupID = 7, SubGroup = "Support Services Combined" }); //50

            Create.Table("NRPRoles")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("NRPSubGroupID").AsInt32().NotNullable().ForeignKey("NRPSubGroups", "ID")
                 .WithColumn("RoleProfile").AsString(255).NotNullable()
            .WithColumn("Active").AsBoolean().NotNullable().WithDefaultValue(true);

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Project Support Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Project Support Officer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Project Support Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Business/Administative Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Business/Administative Manager Higher Level " });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Project Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Improvement and Development Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Commisssioning Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Project Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Operations Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Programme Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Professional Manager, Improvement and Development" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Professional Manager, Performance/Operations" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Professional Manager, Performance/Operations Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 1, RoleProfile = "Professional Manager - corporate level" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Assistant (Higher Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Officer (Higher Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Analyst" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Analyst, Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Section Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Analyst, Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Finance Department Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Principal Finance Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 2, RoleProfile = "Chief Finance Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Assistant Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Administrator" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Advisor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Advisor Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Adviser Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Manager Principal ( Assistant Director)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 3, RoleProfile = "HR Head of service" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Library Technician Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Library Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Library Technician Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Librarian" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Librarian Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Librarian Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Librarian Service Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 4, RoleProfile = "Professional Manager Library Services" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Operator/Telephony Operator" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Analyst/Technician Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Operator Team Leader/Telephony Operator Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Analyst/Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Technician (Statistics/Information Management/Public Health Intelligence)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Analyst (Statistics/Information Management/Public Health Intelligence)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Analyst/Technician Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Analyst Specialist (Statistics/Information Management/Public Health Intelligence)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Analyst Specialist/Technical Engineer/Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Analyst Advanced//Technicial Engineer Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Section Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Analyst, Advanced/Team Manager (Statistics/Information Management/Public Health)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Analyst Principal (Statistics/Information Management/Public Health Intelligence)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 5, RoleProfile = "Information Management and Technnology Service Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 6, RoleProfile = "Solicitor entry level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 6, RoleProfile = "Solicitor principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 6, RoleProfile = "Solicitor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 6, RoleProfile = "Solicitor Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Consumer Services Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Administrator" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Patient Support Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Patient Support Officer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Communications Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Officer Higher Level 1" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Complaints Officer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Communications Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Communicatons Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Service Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Communications Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "PALS Professional Head" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 7, RoleProfile = "Communications Service Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clerical Officer (Data Entry)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Officer Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Assistant Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Team Leader/Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Officer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Officer Higher Level/Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Officer Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Section Manager/Assistant Health Records Manager (Multi Section Unit)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Practice Manager (Small Practice)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Practice Manager (Group Practice)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Multi Section Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Voluntary Services Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Clinical Coding Service Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Department Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 8, RoleProfile = "Health Records Services Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 9, RoleProfile = "Clerical Officer/Receptionist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 9, RoleProfile = "Secretary/Medical Secretary/AdminTeam Leader/Receptionist Higher Level(GP Practice)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 9, RoleProfile = "Secretary Higher Level/Medical Secretary Higher Level/Admin Team coordinator" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 10, RoleProfile = "Arts Therapist Entry Level (Art, Music, Drama, Dance Movement)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 10, RoleProfile = "Arts Therapist (Art, Music, Drama, Dance Movement)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 10, RoleProfile = "Arts Therapist Principal (Art, Music, Drama, Dance Movement)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 10, RoleProfile = "Head of Arts Therapies (Art, Music, Drama, Dance Movement)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 10, RoleProfile = "Arts Therapies Consultant (Art, Music, Drama, Dance Movement)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 11, RoleProfile = "Chaplain Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 11, RoleProfile = "Chaplain" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 11, RoleProfile = "Chaplain Team Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychology, Assistant Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychology, Assistant Practitioner (Higher Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Counsellor Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Primary Care Mental Health Worker (Graduate)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychology Trainee" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Counsellor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Counsellor Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychologist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Counsellor Professional Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Counsellor Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychologist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Pyschologist Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 12, RoleProfile = "Clinical Psychologist Consultant, Professional Lead/Head of Psychology Services" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Clinical Support Worker Higher Level (Dietetics)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Dietitian" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Dietitian Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Dietetic Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Dietitian Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 13, RoleProfile = "Dietitian Specialist (Research)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement, Clerical Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Resource Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Resource Assistant Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Clinical Audit Facilitator/Analyst" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Graphic Designer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Practitioner Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Clinical Audit Facilitator Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Public Health Researcher" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Clinical Governance Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Practitioner Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Public Health Research & Development Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Health Improvement Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Clinical Governance Practitioner (Higher Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 14, RoleProfile = "Public Health Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Optometrist Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Orthoptist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Optometrist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Optometrist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Orthoptists Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Optometrist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Orthoptist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Orthoptists/Optometrist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 15, RoleProfile = "Orthoptist/Optometrist Consultant Head of Service" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Clinical Support Worker (Occupational Therapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Clinical Support Worker Higher Level (Occupational Therapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapy Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapy Technical Instructor Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Advanced (Community)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 16, RoleProfile = "Occupational Therapist Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Clinical Support Worker (Podiatry)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Clinical Support Worker Higher Level (Podiatry)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatry Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatrist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatrist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatrist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatry Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatric Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatric Registrar (Surgery)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatric Consultant (Surgery)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 17, RoleProfile = "Podiatric Consultant (Surgery) Head of Service" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 18, RoleProfile = "Play Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 18, RoleProfile = "Play Specialist Higher Level / Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 18, RoleProfile = "Play Specialist Team Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Clinical Support Worker (Physiotherapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Clinical Support Worker Higher Level (physiotherapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Specialist (Experienced Rotational)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Specialist (Respiratory Problems)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapy Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Specialist (Community)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 19, RoleProfile = "Physiotherapist Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Clinical Support Worker Higher Level (Radiography)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Assistant Practitioner (Radiography)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiography (Therapeutic)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer (Diagnostic)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Specialist (Diagnostic Therapeutic)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Specialist (Reporting Sonographer)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Consultant (Therapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 20, RoleProfile = "Radiographer Consultant (Diagnostic)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 21, RoleProfile = "Sexual Health Adviser" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 21, RoleProfile = "Sexual Health Advisory Service Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Clinical Support Worker (Speech and Language Therapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Clinical Support Worker Higher Level (Speech and Language Therapy)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Speech and Language Therapy Assistant/Associate Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Speech and Language Therapy Associate Practitioner (bi-lingual)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Speech and Language Therapist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Specialist Speech and Language Therapist " });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Speech and Language Therapist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Speech and Language Therapist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 22, RoleProfile = "Consultant Speech and Language Therapist" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Therapy, Assistant Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Practice Education Facilitator (Entry Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Clinical Researcher" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Clinical Researcher Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Practice Education Facilitator" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Clinical Researcher Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "Clinical Researcher" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 23, RoleProfile = "AHP Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre Assistant Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre Practitioner Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre nurse/practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre Practitioner Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre nurse/practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Anaesthesia Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 24, RoleProfile = "Theatre Practitioner Team Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Emergency Services Call Taker" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Patient Transport Services (PTS) Driver" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Ambulance Services Driver (PTS) Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Ambulance Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Ambulance Practitioner Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Paramedic (Newly Qualified)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Paramedic" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Ambulance Practitioner Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Emergency Services Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 25, RoleProfile = "Emergency Services Area Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 26, RoleProfile = "Anatomical Pathology Technician Entry Level (Mortuary)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 26, RoleProfile = "Anatomical Pathology Technician (Mortuary)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 26, RoleProfile = "Anatomical Pathology Technician Higher Level (Mortuary)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Clinical Support Worker (healthcare Science)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Phlebotomist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Clinical Support Worker, Higher Level, (healthcare Science)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Biomedical Scientist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Biomedical Scientist Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Biomedical Scientist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Biomedical Scientist Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 27, RoleProfile = "Biomedical Scientist Advanced" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Medical Physics Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Specialist Medical Physics Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Clinical Scientist (Molecular Genetics/Cytogenetics)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Medical Physics Technician Section Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Principal Clinical Scientist (Molecular Genetics/Cytogenetics)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 28, RoleProfile = "Consultant Clinical Scientist Head of Service (Molecular Genetics/Cytogenetics)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 29, RoleProfile = "Cytology Screener Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 29, RoleProfile = "Cytology Screener" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Dental Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Oral Health Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Dental Technician Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Oral Health Practitioner Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Oral Health Practitioner Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Dental Technician Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 30, RoleProfile = "Dental Laboratory Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 31, RoleProfile = "Genetic Counsellor Trainee" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 31, RoleProfile = "Genetic Counsellor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 31, RoleProfile = "Genetic Counsellor Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 31, RoleProfile = "Genetic Counsellor Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Assistant Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Associate Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Practitioner Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Practitioner Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Practitioner Advanced (Research)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Practitioner Principal (Research)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Science Service Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Graduate Trainee" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist (Research)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Specialist (Research)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Advanced " });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Head of Service/Director" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 32, RoleProfile = "Healthcare Scientist Consultant" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 33, RoleProfile = "Physican Assocate Entry Level " });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 33, RoleProfile = "Physician Associate" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Medical Engineering Technician, Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Medical Engineering Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Physiological Measurement Practitioner/Clinical Physiologist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Medical Engineering Technician Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Specialist Physiological Measurement Practitioner/Clinical Physiologist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Highly Specialist Physiological Measurement Practitioner/Clinical Physiologist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Medical Engineering Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Physiological Measurement/Clinical Physiology Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 34, RoleProfile = "Physiological Measurement/Clinical Physiology Service Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Optometrist Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Orthoptist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Optometrist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Optometrist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Orthoptists Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Optometrist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Orthoptist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Orthoptists/Optometrist Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 35, RoleProfile = "Orthoptist/Optometrist Consultant Head of Service" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Support Worker" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Support Worker, Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Technician" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacist Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Technician Higher Level (Pharmacy or Primary Care)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Technician Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Technician Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacist Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacist Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacist Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Pharmacy Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 36, RoleProfile = "Professional Manager Pharmaceutical Services" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 37, RoleProfile = "Screening Practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 37, RoleProfile = "Screening Practitioner Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 37, RoleProfile = "Screening Practitioner, Service Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Clinical Support Worker" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Clinical Support Worker, Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Associate Practitioner/Nursery Nurse" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Team Leader (Learning Disabilities)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Advanced" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Modern Matron" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 38, RoleProfile = "Nurse/Midwife Consultant Higher Level" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Clinical Support Worker (Dentistry)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse Higher Level " });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 39, RoleProfile = "Dental Nurse Tutor" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Practice Education Facilitator (Entry Level)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Clinical Researcher" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Practic Education Facilitator" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Clinical Researcher Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Clinical Researcher Principal" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 40, RoleProfile = "Clinical Researcher" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 41, RoleProfile = "Health Visitor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 41, RoleProfile = "Health Visitor Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 41, RoleProfile = "Health Visitor Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 41, RoleProfile = "Nursing/Health Visitor Specialist (Community Practice Teacher)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Maternity Care Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife (Community)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife (Hospital)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife (Integrated)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife Higher Level (Research Projects)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Midwife Consultant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 42, RoleProfile = "Nurse/Midwife Consultant Higher Level" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 43, RoleProfile = "Nurse Specialist (NHS Direct)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 43, RoleProfile = "Nurse Team Manager (NHS Direct)" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 44, RoleProfile = "Theatre Nurse" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 44, RoleProfile = "Theatre nurse/practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 44, RoleProfile = "Theatre nursse/practitioner" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 44, RoleProfile = "Theatre Nurse Specialist" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 45, RoleProfile = "Professional Manager (Clinical, Clinical Technical Service) 8A" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 45, RoleProfile = "Professional Manager (Clinical, Clinical Technical Service) 8B" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 45, RoleProfile = "Professional Manager (Clinical, Clinical Technical Service) 8C" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Home Carer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Home Carer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Home Care Organiser (Client Assessment)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Home Care Organiser (Staff)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Home Care Team Leader ( Staff Supervision and Client Assessment)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 46, RoleProfile = "Care Coordinator" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Care Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Driver with Caring Duties" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Residential Carer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Day Centre Carer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Social Work Assistant Practitioner (Residential)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Day Centre Carer (Higher Level/Team Leader)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Residential Carer Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Social Work Residential" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 47, RoleProfile = "Day Centre Manager" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Work Team Manager (Adult Residential)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Work Entry Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Rehabilitation Worker (Sensory Impairment)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Worker" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Work Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Worker Specialist" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Work Locality/Service Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 48, RoleProfile = "Social Care Programme Manager/Assistant Director Social Services" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Support Worker" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Support Worker Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Tradesperson" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Tradesperson Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Tradesperson Team Leader" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Officer (Operations)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Officer (Specialist Services)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Officer (Projects)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Manager (Operations)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Manager (Specialist Services)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Manager (Projects)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Manager higher level (Projects)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Estates Manager Higher Level (Operations/Specialist Services)" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Head of Estates/Assistant Head of Estates" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 49, RoleProfile = "Director of Estates and Facilities" });

            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Support Worker Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Catering Assistant" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Support Service Supervisor" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Security Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Cook" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Cook Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Supplies and Procurement Administrative Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Catering Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Procurement Officer" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Catering Manager Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Procurement Officer Higher Level" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Support Services Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Procurement Team Manager" });
            Insert.IntoTable("NRPRoles").Row(new { NRPSubGroupID = 50, RoleProfile = "Head of Procurement and Supply" });

            Alter.Table("RoleProfiles").AddColumn("NRPProfessionalGroupID").AsInt32().Nullable().ForeignKey("NRPProfessionalGroups", "ID");
            Alter.Table("RoleProfiles").AddColumn("NRPSubGroupID").AsInt32().Nullable().ForeignKey("NRPSubGroups", "ID");
            Alter.Table("RoleProfiles").AddColumn("NRPRoleID").AsInt32().Nullable().ForeignKey("NRPRoles", "ID");
            Alter.Table("RoleProfiles").AddColumn("PublishStatusID").AsInt32().NotNullable().ForeignKey("PublishStatus", "ID").WithDefaultValue(1);

            Delete.ForeignKey("FK_RoleProfiles_CategoryID_CourseCategories_CourseCategoryID").OnTable("RoleProfiles");
            Delete.ForeignKey("FK_RoleProfiles_TopicID_CourseTopics_CourseTopicID").OnTable("RoleProfiles");
            Delete.Column("CategoryID").FromTable("RoleProfiles");
            Delete.Column("TopicID").FromTable("RoleProfiles");
        }

        public override void Down()
        {
            Delete.Column("NRPProfessionalGroupID").FromTable("RoleProfiles");
            Delete.Column("NRPSubGroupID").FromTable("RoleProfiles");
            Delete.Column("NRPRoleID").FromTable("RoleProfiles");

            Alter.Table("RoleProfiles").AddColumn("CategoryID").AsInt32().NotNullable().ForeignKey("CourseCategories", "CourseCategoryID").WithDefaultValue(1);
            Alter.Table("RoleProfiles").AddColumn("TopicID").AsInt32().NotNullable().ForeignKey("CourseTopics", "CourseTopicID").WithDefaultValue(1);

            Delete.Table("NRPRoles");
            Delete.Table("NRPSubGroups");
            Delete.Table("NRPProfessionalGroups");
        }

    }
}
