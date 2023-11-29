using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HSV
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public List<Consultation> Consultations { get; set; } = new List<Consultation>();
    }

    public class Consultation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public int DoctorId { get; set; }
        public string PatientName { get; set; }
        public int Status { get; set; } // Add Status property (0: Pending, 1: Accepted, -1: Declined)
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
    }

    public class AppointmentManager
    {
        private List<Patient> patients = new List<Patient>();
        private List<Doctor> doctors = new List<Doctor>();
        private List<Consultation> consultations = new List<Consultation>();
        private string dataFilePath = "appointments.json";
        private string doctorDataFilePath = "doctor.json";
        private string patientDataFilePath = "patients.json";

        public AppointmentManager()
        {
            LoadDataFromFile();
            LoadDoctorsFromJson();
        }

        private void LoadDoctorsFromJson()
        {
            try
            {
                if (File.Exists(doctorDataFilePath))
                {
                    string jsonData = File.ReadAllText(doctorDataFilePath);
                    doctors = JsonConvert.DeserializeObject<List<Doctor>>(jsonData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctor data from the file: {ex.Message}");
            }
        }

        public List<Consultation> GetConsultationsForPatient(Patient patient)
        {
            return consultations.FindAll(c => c.PatientName == (patient.FirstName + " " + patient.LastName));
        }

        public Doctor GetDoctorById(int doctorId)
        {
            return doctors.Find(d => d.Id == doctorId);
        }

        private void LoadDataFromFile()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    string jsonData = File.ReadAllText(dataFilePath);
                    var data = JsonConvert.DeserializeObject<Data>(jsonData);
                    patients = data.Patients;
                    consultations = data.Consultations;
                }

                if (File.Exists(doctorDataFilePath))
                {
                    string jsonDoctorData = File.ReadAllText(doctorDataFilePath);
                    doctors = JsonConvert.DeserializeObject<List<Doctor>>(jsonDoctorData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from the file: {ex.Message}");
            }
        }

        public void AddPatient(Patient patient)
        {
            patient.Id = GetNextPatientId();
            patients.Add(patient);
            SaveAppointments();
        }

        public void AddDoctor(Doctor doctor)
        {
            doctor.Id = GetNextDoctorId();
            doctors.Add(doctor);
            SaveAppointments();
        }

        public void AddConsultation(Consultation consultation)
        {
            consultation.Id = GetNextConsultationId();
            consultation.Status = 0; // Set the initial status to Pending (0)
            consultations.Add(consultation);
            SaveAppointments();
        }

        private int GetNextPatientId()
        {
            return patients.Count > 0 ? patients.Max(p => p.Id) + 1 : 1;
        }

        private int GetNextDoctorId()
        {
            return doctors.Count > 0 ? doctors.Max(d => d.Id) + 1 : 1;
        }

        private int GetNextConsultationId()
        {
            return consultations.Count > 0 ? consultations.Max(c => c.Id) + 1 : 1;
        }

        public void LoadAppointments()
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                var data = JsonConvert.DeserializeObject<Data>(json);
                patients = data.Patients;
                doctors = data.Doctors;
                consultations = data.Consultations;
            }
        }

        public void SaveAppointments()
        {
            var data = new Data
            {
                Patients = patients,
                Doctors = doctors,
                Consultations = consultations
            };
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(dataFilePath, json);
            File.WriteAllText(doctorDataFilePath, JsonConvert.SerializeObject(doctors));
            File.WriteAllText(patientDataFilePath, JsonConvert.SerializeObject(patients));
        }

        public List<Patient> GetPatients()
        {
            return patients;
        }

        public List<Doctor> GetDoctors()
        {
            return doctors;
        }

        public List<Consultation> GetConsultations()
        {
            return consultations;
        }

        public List<Consultation> GetConsultationsForDoctor(Doctor doctor)
        {
            return consultations.FindAll(c => c.DoctorId == doctor.Id);
        }

        public List<string> GetDoctorSpecializations()
        {
            List<string> specializations = new List<string>();

            foreach (var doctor in doctors)
            {
                if (!specializations.Contains(doctor.Specialization))
                {
                    specializations.Add(doctor.Specialization);
                }
            }

            return specializations;
        }

        private class Data
        {
            public List<Patient> Patients { get; set; }
            public List<Doctor> Doctors { get; set; }
            public List<Consultation> Consultations { get; set; }
        }
    }
}
