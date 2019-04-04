using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;   // make sure you add Newtonsoft.JSON as a project dependency

namespace Interview.Problem1.Domain
{
    public class myClass
    {

        public static void Main()
        {

            TripVm alarmTrips;
            using (StreamReader file = File.OpenText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"/tripdata.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                alarmTrips = (TripVm)serializer.Deserialize(file, typeof(TripVm));
            }
            List<DateTime> uploadList = new List<DateTime>();
            List<DateTime> timeList = new List<DateTime>();
            for (var i = 0; i<alarmTrips.TripSettings.Count; i++) {
                var events = from x in alarmTrips.TripUploadData
                             where x.Channel.Equals(alarmTrips.TripSettings[i].ChannelName) && (x.Data < alarmTrips.TripSettings[i].Min || x.Data > alarmTrips.TripSettings[i].Max)
                             select x.Timestamp;

                foreach (var x in events)
                    timeList.Add(x);
                
                
            }
            timeList = timeList.OrderBy(x => x.TimeOfDay).ToList();
            timeList.ForEach(el => Console.WriteLine(el));
            Console.WriteLine("");

            foreach (var TrippedEvents in alarmTrips.TripUploadEvents)
            {
                uploadList.Add(TrippedEvents.Timestamp);
            }
            uploadList = uploadList.OrderBy(x => x.TimeOfDay).ToList();
            uploadList.ForEach(el => Console.WriteLine(el));



            /*
                        foreach (var TrippedEvents in alarmTrips.TripUploadEvents)
                        {
                            eventList.Add(TrippedEvents);
                        }
                        //eventList.ForEach(el => Console.WriteLine(el.Timestamp));
                        //var difference=0;
                        int j = 0;
                        //if (eventList.Count % 2 != 0 ) //data collection stopped: Use end time for last element
                        //{

                        for (int i = 0; i < eventList.Count - 1; i++)                 //count-2 if odd number of elements in collected data
                        {
                            Console.WriteLine("Duration of alarms insdie of the time data was collected " + eventList[j + i].Timestamp.Subtract(eventList[j + i + 1].Timestamp) + ". Channel Name: " + eventList[j + i].Channel);
                            j++;

                        }
                        //Console.WriteLine(alarmTrips.EndTime.Subtract(eventList[eventList.Count - 1].Timestamp)); 
                        //}
                        var sensor1eventList = new List<DeviceDataVm>();
                        foreach (var unTrippedEvents in alarmTrips.TripUploadData) // for sensor 1
                        {
                            if (string.Equals(unTrippedEvents.Channel, alarmTrips.TripSettings[0].ChannelName))
                            {
                                if (unTrippedEvents.Data < alarmTrips.TripSettings[0].Min || unTrippedEvents.Data > alarmTrips.TripSettings[0].Max)
                                {
                                    Console.WriteLine(unTrippedEvents.Data);
                                }
                            }
                        }

                        var sensor2eventList = new List<DeviceDataVm>();
                        foreach (var unTrippedEvents in alarmTrips.TripUploadData) // for sensor 2
                        {
                            if (string.Equals(unTrippedEvents.Channel, alarmTrips.TripSettings[1].ChannelName))
                            {
                                if (unTrippedEvents.Data < alarmTrips.TripSettings[1].Min || unTrippedEvents.Data > alarmTrips.TripSettings[1].Max)
                                {
                                    //Console.WriteLine(unTrippedEvents.Data);
                                    sensor2eventList.Add(unTrippedEvents);
                                }
                                if (unTrippedEvents.Data > alarmTrips.TripSettings[1].Min && unTrippedEvents.Data < alarmTrips.TripSettings[1].Max && sensor2eventList.Count() != 0)
                                {
                                    Console.WriteLine("Duration of alarms outside of the time data was collected " + sensor2eventList[0].Timestamp.Subtract(sensor2eventList[sensor2eventList.Count - 1].Timestamp) + ". Channel Name: " + sensor2eventList[sensor2eventList.Count - 1].Channel);
                                    //sensor2eventList.ForEach(el => Console.WriteLine(el.Timestamp));
                                    sensor2eventList.Clear();
                                }
                            }
                        }
                        //sensor2eventList.ForEach(el => Console.WriteLine(el.Timestamp));
                   }*/
        }

        public enum AlarmState
        {
            /// <summary>
            /// Indicates device alarm when channel reading has gone out of the Min/Max range
            /// </summary>
            AlarmOut = 0,

            /// <summary>
            /// Indicates device alarm when channel reading has returned within the Min/Max range
            /// </summary>
            AlarmIn,

            /// <summary>
            /// Indicates that an alarm has been acknowledged on the device
            /// </summary>
            AlarmAcknowledge,

            /// <summary>
            /// Indicates a low battery alarm
            /// </summary>
            LowBattery,

            /// <summary>
            /// Indicates a potential lost connectivity condition. This alarm is thrown
            /// by the cloud service after 2 missed update intervals.
            /// </summary>
            LostConnectivity,

            /// <summary>
            /// Indicates no alarm on the channel
            /// </summary>
            NoAlarm,

            /// <summary>
            /// Indicates when channel reading has exceeded the device Max alarm setting
            /// </summary>
            MaxAlarmOut,

            /// <summary>
            /// Indicates when channel reading has exceeded the device Min alaram setting
            /// </summary>
            MinAlarmOut,

            /// <summary>
            /// Indicates when the channel reading has reentered the valid range from outside the upper bound
            /// </summary>
            MaxAlarmIn,

            /// <summary>
            /// Indicates when the channel reading has reentered the valid range from outside the lower bound
            /// </summary>
            MinAlarmIn,

            /// <summary>
            /// Indicates when device connectivity is restored
            /// </summary>
            ConnectvityRestored,

            /// <summary>
            /// Indicates when a channel's alarm setting has been changed
            /// </summary>
            SettingChanged,

            /// <summary>
            /// Indicates that a user has physically checked the device by pushing the alarm acknowledge button
            /// while the device is not in alarm.
            /// </summary>
            DeviceChecked,

            /// <summary>
            /// Indicates that a user has pushed the start button on a non-connected logging device to commence data logging
            /// </summary>
            LoggingStarted,

        }

        public abstract class ViewModelBase
        {
            public const long InvalidId = 0;

            /// <summary>
            /// Database id field for object
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// Audit column recording time object created in database
            /// </summary>
            public DateTime? CreatedAt { get; set; }

            /// <summary>
            /// Audit column recording the name of the user who saved the object
            /// </summary>
            public string CreatedBy { get; set; }

            /// <summary>
            /// Audit column recording time object modified in database 
            /// </summary>
            public DateTime? ModifiedAt { get; set; }

            /// <summary>
            /// Audit column recording the name of the user who modified the object
            /// </summary>
            public string ModifiedBy { get; set; }

            /// <summary>
            /// Method to stringify the object as a JSON string
            /// </summary>
            /// <returns></returns>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public abstract class DocViewModelBase
        {
            /// <summary>
            /// Database id field for object
            /// </summary>
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            /// <summary>
            /// Audit column recording time object created in database
            /// </summary>
            public DateTime? CreatedAt { get; set; }

            /// <summary>
            /// Audit column recording the name of the user who saved the object
            /// </summary>
            public string CreatedBy { get; set; }

            /// <summary>
            /// Audit column recording time object modified in database 
            /// </summary>
            public DateTime? ModifiedAt { get; set; }

            /// <summary>
            /// Audit column recording the name of the user who modified the object
            /// </summary>
            public string ModifiedBy { get; set; }

            /// <summary>
            /// Method to stringify the object as a JSON string
            /// </summary>
            /// <returns></returns>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// Alarm comment thread item. 
        /// </summary>
        public class AlarmCommentThreadItemVm
        {
            public string Author { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Content { get; set; }
        }

        public class DeviceAlarmVm : ViewModelBase
        {
            /// <summary>
            /// Account id for the device 
            /// </summary>
            public long AccountId { get; set; }

            /// <summary>
            /// Device serial number
            /// </summary>
            public string SerialNumber { get; set; }

            /// <summary>
            /// Device channel triggering the alarm
            /// </summary>
            public string Channel { get; set; }

            /// <summary>
            /// Alarm event type 
            /// 
            /// 0 - ALARM OUT
            /// 1 - ALARM IN
            /// 2 - ALARM ACK
            /// 3 - LOW BATTERY
            /// 4 - LOST CONNECTIVITY
            /// 5 - NO ALARM
            /// 6 - MAX ALARM OUT
            /// 7 - MIN ALARM OUT
            /// 8 - MAX ALARM IN
            /// 9 - MIN ALARM IN
            /// 10 - CONNECTIVITY RESTORED
            /// 11 - SETTING CHANGED

            /// 
            /// </summary>
            public int EventType { get; set; }

            /// <summary>
            /// Timestamp of the alarm event
            /// </summary>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Data associated with the alarm
            /// </summary>
            public string AlarmData { get; set; }

            /// <summary>
            /// Comments associated with the alarm
            /// </summary>
            public string Comments { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<AlarmCommentThreadItemVm> CommentThread { get; set; }

            /// <summary>
            /// Measurement data type on this channel
            /// </summary>
            public string DataType { get; set; }

            /// <summary>
            /// Unit label for the channel reading based on user preferences
            /// </summary>
            public string UnitLabel { get; set; }
        }

        public class DeviceDataVm : ViewModelBase
        {
            /// <summary>
            /// Account id for the device 
            /// </summary>
            public long? AccountId { get; set; }

            /// <summary>
            /// Device channel id
            /// </summary>
            public string Channel { get; set; }

            /// <summary>
            /// Device serial number
            /// </summary>
            public string SerialNumber { get; set; }

            /// <summary>
            /// Timestamp when data was recorded by device
            /// </summary>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Device measurement data
            /// </summary>
            public double Data { get; set; }
        }

        /// <summary>
        /// Object used for associating device data with a particular location and period in time
        /// </summary>
        public class TripVm : DocViewModelBase
        {
            /// <summary>
            /// User account associated with this data
            /// </summary>
            public long AccountId { get; set; }

            /// <summary>
            /// Serial number of the device associated with this trip
            /// </summary>
            public string SerialNumber { get; set; }

            /// <summary>
            /// Model id of the datalogger used for this trip
            /// </summary>
            public long DeviceModelId { get; set; }

            /// <summary>
            /// Name of the trip
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Start time of the trip
            /// </summary>
            public DateTime StartTime { get; set; }

            /// <summary>
            /// End time of the trip
            /// </summary>
            public DateTime EndTime { get; set; }

            /// <summary>
            /// Logging interval (in minutes) for the device during this trip
            /// </summary>
            public int LoggingInterval { get; set; }

            /// <summary>
            /// Enable alarms during this trip
            /// </summary>
            public bool IsAlarmEnabled { get; set; }


            /// <summary>
            /// Check if the Trip Alarm events is wrapped
            /// </summary>
            public bool IsAlarmWrapped { get; set; }


            /// <summary>
            /// The location associated with this trip. Can be either a transport or physical location
            /// </summary>
            public long LocationId { get; set; }

            /// <summary>
            /// The list of data logger channel settings for this trip. (Required)
            /// </summary>
            public List<TripSettingVm> TripSettings { get; set; } = new List<TripSettingVm>();

            /// <summary>
            /// Flag to denote logical deletion
            /// </summary>
            public bool IsDeleted { get; set; } = false;

            /// <summary>
            /// The uploaded trip data from the device
            /// </summary>
            public List<DeviceDataVm> TripUploadData { get; set; }

            /// <summary>
            /// uploaded trip alarm data from the device
            /// </summary>
            public List<DeviceAlarmVm> TripUploadEvents { get; set; }
        }

        public class TripSettingVm
        {
            /// <summary>
            /// Account id associated with the data
            /// </summary>
            public long AccountId { get; set; }

            /// <summary>
            /// Id of the trip for this setting
            /// </summary>
            public string TripId { get; set; }

            /// <summary>
            /// Data type for a specific channel on the data logger
            /// </summary>
            public string DataType { get; set; }

            /// <summary>
            /// Min alarm setting for the channel on this trip
            /// </summary>
            public double Min { get; set; }

            /// <summary>
            /// Max alarm setting for the channel on this trip
            /// </summary>
            public double Max { get; set; }

            /// <summary>
            /// Name of the channel (sensor1, sensor2, etc.)
            /// </summary>
            public string ChannelName { get; set; }

            /// <summary>
            /// User specified alias for the channel
            /// </summary>
            public string Alias { get; set; }
        }
    }
}