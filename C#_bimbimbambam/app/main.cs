using System;

namespace HSV
{
    class Program
    {
        static void Main(string[] args)
        {
            AppointmentManager appointmentManager = new AppointmentManager();

            while (true)
            {
                Console.WriteLine("Select an action:");
                Console.WriteLine("1. Log in as a patient");
                Console.WriteLine("2. Log in as a doctor");
                Console.WriteLine("3. Exit");
                Console.WriteLine("4. Register");

                string choice = Console.ReadLine();
                                switch (choice)
                {
                    case "1":
                        Patient patient = LoginAsPatient(appointmentManager);
                        if (patient != null)
                        {
                            Console.WriteLine($"You are logged in as a patient: {patient.FirstName} {patient.LastName}");
                            while (true)
                            {
                                Console.WriteLine("Patient Menu:");
                                Console.WriteLine("1. View Consultations");
                                Console.WriteLine("2. Schedule Appointment");
                                Console.WriteLine("3. Go Back");

                                string patientOption = Console.ReadLine();

                                switch (patientOption)
                                {
                                    case "1":
                                        ViewConsultations(patient, appointmentManager);
                                        break;
                                    case "2":
                                        ScheduleAppointment(patient, appointmentManager);
                                        break;
                                    case "3":
                                        return; // Go back to the main menu
                                    default:
                                        Console.WriteLine("Invalid choice. Please try again.");
                                        break;
                                }
                            }
                        }
                        break;
                    case "2":
                        Doctor doctor = LoginAsDoctor(appointmentManager);
                        if (doctor != null)
                        {
                            Console.WriteLine($"You are logged in as a doctor: {doctor.FirstName} {doctor.LastName}, Specialization: {doctor.Specialization}");
                            while (true)
                            {
                                Console.WriteLine("Doctor Menu:");
                                Console.WriteLine("1. View Consultations");
                                Console.WriteLine("2. Manage Consultations");
                                Console.WriteLine("3. Go Back");

                                string doctorOption = Console.ReadLine();

                                switch (doctorOption)
                                {
                                    case "1":
                                        ListConsultationsForDoctor(doctor, appointmentManager);
                                        break;
                                    case "2":
                                        ManageConsultations(doctor, appointmentManager);
                                        break;
                                    case "3":
                                        return; // Go back to the main menu
                                    default:
                                        Console.WriteLine("Invalid choice. Please try again.");
                                        break;
                                }
                            }
                        }
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    case "4":
                        RegisterPatient(appointmentManager);
                        Console.WriteLine("Registration successful. Press [ENTER] to continue.");
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static bool CheckPassword(Patient patient, string password)
        {
        // You can implement password verification logic here, such as comparing it with a stored hash.
        // In this example, the password is stored in plain text for simplicity.
        return patient.Password == password;
        }
        static Patient LoginAsPatient(AppointmentManager appointmentManager)
        {
            Console.WriteLine("Enter the patient's first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter the patient's last name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Enter the password:");

            string password = "";
            while (true)
            {
                var key = Console.ReadKey(true); // true - hides input on the screen
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Move to the next line after entering the password
                    break;
                }
                password += key.KeyChar;
            }

            var patients = appointmentManager.GetPatients();
            var patient = patients.Find(p => p.FirstName == firstName && p.LastName == lastName);

            if (patient != null && CheckPassword(patient, password))
            {
                return patient;
            }
            else
            {
                Console.WriteLine("Incorrect name, last name, or password.");
                Console.WriteLine("Press [R] to register. Press any other key to continue.");
                var choice = Console.ReadKey();
                if (choice.Key == ConsoleKey.R)
                {
                    RegisterPatient(appointmentManager);
                }
                return null;
            }
        }

        static Doctor LoginAsDoctor(AppointmentManager appointmentManager)
        {
            Console.WriteLine("Enter the doctor's first name:");
            string doctorFirstName = Console.ReadLine();

            Console.WriteLine("Enter the doctor's last name:");
            string doctorLastName = Console.ReadLine();

            // Check if a doctor with the specified name and last name exists
            var doctors = appointmentManager.GetDoctors();
            var doctor = doctors.Find(d => d.FirstName == doctorFirstName && d.LastName == doctorLastName);
            if (doctor != null)
            {
                return doctor;
            }
            else
            {
                Console.WriteLine("Doctor with that name and last name not found.");
                return null;
            }
        }

        static void ViewConsultations(Patient patient, AppointmentManager appointmentManager)
        {
            var consultations = appointmentManager.GetConsultationsForPatient(patient);
            if (consultations.Count > 0)
            {
                Console.WriteLine($"Consultations for {patient.FirstName} {patient.LastName}:");
                foreach (var consultation in consultations)
                {
                    var doctor = appointmentManager.GetDoctorById(consultation.DoctorId);
                    Console.WriteLine($"Doctor: {doctor.FirstName} {doctor.LastName}, Specialization: {doctor.Specialization}");
                    Console.WriteLine($"Date: {consultation.Date.ToShortDateString()}");
                    Console.WriteLine($"Status: {GetConsultationStatus(consultation.Status)}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No consultations found for this patient.");
            }
        }

        static void ScheduleAppointment(Patient patient, AppointmentManager appointmentManager)
{
    Console.WriteLine("Select a doctor's specialization from the list:");
    var doctorSpecializations = appointmentManager.GetDoctorSpecializations();
    
    foreach (var specialization in doctorSpecializations)
    {
        Console.WriteLine(specialization);
    }

    string selectedSpecialization = Console.ReadLine();
    
    var doctors = appointmentManager.GetDoctors();
    var doctor = doctors.Find(d => d.Specialization == selectedSpecialization);
    
    if (doctor != null)
    {
        Console.WriteLine("Enter the consultation date (yyyy-mm-dd):");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime consultationDate))
        {
            Consultation consultation = new Consultation
            {
                Name = doctor.FirstName + " " + doctor.LastName,
                Date = consultationDate,
                DoctorId = doctor.Id,
                PatientName = patient.FirstName + " " + patient.LastName,
                Status = 0 // Set the status to 0 for pending
            };

            appointmentManager.AddConsultation(consultation);
            Console.WriteLine("Consultation request submitted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Please try again.");
        }
    }
    else
    {
        Console.WriteLine("Doctor with the selected specialization not found.");
    }
}


        static string GetConsultationStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return "Pending";
                case 1:
                    return "Accepted";
                case -1:
                    return "Declined";
                default:
                    return "Unknown";
            }
        }

        static void ListConsultationsForDoctor(Doctor doctor, AppointmentManager appointmentManager)
        {
            var consultations = appointmentManager.GetConsultationsForDoctor(doctor);
            Console.WriteLine($"List of pending consultations for Dr. {doctor.FirstName} {doctor.LastName}, Specialization: {doctor.Specialization}:");

            foreach (var consultation in consultations)
            {
                if (consultation.Status == 0)
                {
                    Console.WriteLine($"ID: {consultation.Id}, Patient: {consultation.PatientName}, Date: {consultation.Date.ToShortDateString()}, Status: Pending");
                }
            }
        }

        static void ManageConsultations(Doctor doctor, AppointmentManager appointmentManager)
        {
            var consultations = appointmentManager.GetConsultationsForDoctor(doctor);

            if (consultations.Count > 0)
            {
                Console.WriteLine($"List of pending consultations for Dr. {doctor.FirstName} {doctor.LastName}, Specialization: {doctor.Specialization}:");

                foreach (var consultation in consultations)
                {
                    if (consultation.Status == 0)
                    {
                        Console.WriteLine($"ID: {consultation.Id}, Patient: {consultation.PatientName}, Date: {consultation.Date.ToShortDateString()}, Status: Pending");
                    }
                }

                Console.WriteLine("Enter the ID of the consultation to manage (or type '0' to go back):");
                if (int.TryParse(Console.ReadLine(), out int consultationId))
                {
                    if (consultationId == 0)
                    {
                        return; // Go back to the doctor menu
                    }

                    var selectedConsultation = consultations.Find(c => c.Id == consultationId);

                    if (selectedConsultation != null)
                    {
                        Console.WriteLine($"Selected Consultation: ID: {selectedConsultation.Id}, Patient: {selectedConsultation.PatientName}, Date: {selectedConsultation.Date.ToShortDateString()}");

                        Console.WriteLine("Choose an action:");
                        Console.WriteLine("1. Accept Consultation");
                        Console.WriteLine("2. Decline Consultation");
                        Console.WriteLine("3. Go Back");

                        string actionChoice = Console.ReadLine();

                        switch (actionChoice)
                        {
                            case "1":
                                selectedConsultation.Status = 1;
                                appointmentManager.SaveAppointments();
                                Console.WriteLine("Consultation accepted.");
                                break;
                            case "2":
                                selectedConsultation.Status = -1;
                                appointmentManager.SaveAppointments();
                                Console.WriteLine("Consultation declined.");
                                break;
                            case "3":
                                return; // Go back to the doctor menu
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Consultation with the specified ID not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid consultation ID.");
                }
            }
            else
            {
                Console.WriteLine("No pending consultations found for this doctor.");
            }
        }
        static void RegisterPatient(AppointmentManager appointmentManager)
        {
            Console.WriteLine("Enter the patient's first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter the patient's last name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Enter the password:");
            string password = ReadPassword();

            Console.WriteLine("Confirm the password:");
            string confirmPassword = ReadPassword();

            if (password != confirmPassword)
            {
                Console.WriteLine("Passwords do not match. Registration not completed.");
                return;
            }

            // Check if a patient with the same name and last name already exists
            var patients = appointmentManager.GetPatients();
            var existingPatient = patients.Find(p => p.FirstName == firstName && p.LastName == lastName);

            if (existingPatient != null)
            {
                Console.WriteLine("A patient with that name and last name already exists. Registration not completed.");
                return;
            }

            // Create a new patient and add them to the list of patients
            var newPatient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                Password = password // Store the password
            };

            appointmentManager.AddPatient(newPatient);
            Console.WriteLine("Registration completed successfully.");
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password;
        }
    }
}
