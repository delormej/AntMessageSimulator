using AntMessageSimulator.Messages.Speed;
using AntMessageSimulator.Messages.Fec;
using AntMessageSimulator.Messages.BikePower;

namespace AntMessageSimulator
{    
    public record Reading (
        double Timestamp,
        double Power,
        double SpeedMps,
        ushort ServoPosition,
        double Acceleration,
        ushort TargetPower
    );

    public class CreateReadingVisitor
    {
        private Reading lastReading;
        private GeneralFEDataMessage lastGeneralFEDataMessage;

        public CreateReadingVisitor()
        {
            lastReading = new Reading(0, 0, 0, 0, 0, 0);
        }

        public Reading Visit(StandardCrankTorque message)
        {
            Reading reading = lastReading with {
                Timestamp = message.Timestamp,
                Power = message.CalculatedPower
            };

            lastReading = reading;
            return lastReading;
        }

        public Reading Visit(IrtExtraInfoMessage message)
        {
            Reading reading = lastReading with {
                Timestamp = message.Timestamp,
                ServoPosition = message.ServoPosition,
                TargetPower = message.Target
            };

            lastReading = reading;
            return lastReading;
        }

        public Reading Visit(GeneralFEDataMessage message)
        {
            Reading reading = lastReading with {
                Timestamp = message.Timestamp,
                SpeedMps = message.Speed,
                Acceleration = CalculateAcceleration(message)
            };

            lastReading = reading;
            lastGeneralFEDataMessage = message;
            return lastReading;
        }

        private double CalculateAcceleration(GeneralFEDataMessage message)
        {
            if (lastGeneralFEDataMessage == null)
                return 0;
            
            double dTime = message.Timestamp - lastGeneralFEDataMessage.Timestamp;
            double dSpeed = message.Speed - lastGeneralFEDataMessage.Speed;
            
            if (dTime == 0 || dSpeed == 0)
                return 0;

            return dTime / dSpeed;
        }

        // I'm not sure we actually collect speed, it's in the GeneralFEDataMessage message.
        public Reading Visit(BikeSpeedMessage message)
        {
            return null;

            // Reading reading = lastReading with {
            //     Timestamp = message.Timestamp,
            //     SpeedMps = CalculateSpeed(message)
            // };

            // lastReading = reading;
            // lastBikeSpeedMessage = message;
            // return lastReading;
        }
/*
        private double CalculateSpeed(BikeSpeedMessage message)
        {
            if (lastBikeSpeedMessage == null)
                return 0;

            double dTime = CalculateTime(message);
            double distance = CalculateDistance(message);
            
            double speed = distance / dTime;
            return speed;
        }        

        /// <summary>
        /// Calculates distance in meters.
        /// </summary>
        private double CalculateDistance(BikeSpeedMessage message)
        {
            double revCount = RevCount(message.RevCount, lastBikeSpeedMessage.RevCount);
            double distanceM = ((revCount * wheelSizeMM) / 1000.0);
            return distanceM;
        }

        private ushort RevCount(ushort current, ushort last)
        {
            int value; 
            if (last > current) 
                // rollover
                value = (ushort.MaxValue ^ last) + current;
            else
                value = current - last;
            return (ushort)value;                        
        }

        private double CalculateTime(BikeSpeedMessage message)
        {
            // Each represents 1/1024 second
            return 0;
        }

        // 
        // Unfortunately as of 12/6/2020 current firmware version is sending GAP Offset
        // in the revs field instead of speed.
        
        private double CalculateSpeed(IrtExtraInfoMessage message)
        {
            if (lastIrtExtraInfoMessage == null)
                return 0;

            double dTime = message.Timestamp - lastIrtExtraInfoMessage.Timestamp;
            ushort dFlywheel = FlywheelDelta(message.FlyWheel, lastIrtExtraInfoMessage.FlyWheel);
            double distance = DistanceFromFlywheel(dFlywheel);
            
            double speed = distance / dTime;
            return speed;
        }

        private double DistanceFromFlywheel(int rotations)
        {
            const int FLYWHEEL_TICK_PER_REV = 2;																    // # of ticks per flywheel revolution.
            const float FLYWHEEL_ROAD_DISTANCE = 0.115f;															// Virtual road distance traveled in meters per complete flywheel rev.
            const float FLYWHEEL_TICK_PER_METER = ((1.0f / FLYWHEEL_ROAD_DISTANCE) * FLYWHEEL_TICK_PER_REV);		// # of flywheel ticks per meter

            return rotations / FLYWHEEL_TICK_PER_METER;
        }

        private ushort FlywheelDelta(ushort current, ushort last)
        {
            int value; 
            if (last > current) 
                // rollover
                value = (ushort.MaxValue ^ last) + current;
            else
                value = current - last;
            return (ushort)value;                        
        }
        */
    }
}