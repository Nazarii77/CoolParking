// TODO: implement class TimerService from the ITimerService interface.
//       Service have to be just wrapper on System Timers.

using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;
using System;

namespace CoolParking.BL.Services
{

    public class TimeService : ITimerService
    {
        private readonly Parking parking = Parking.GetParking();
        ILogService LogService { get; set; }
        public double Interval { get; set; }

        public event ElapsedEventHandler Elapsed;
        private Timer TimerPay { get; set; }
        public Timer TimerLog { get; set; }
        public TimeService()
        {
           
            Elapsed += (sender, e) => RegularWithdraw(sender, e);
            LogService = new LogService(Settings.LogFilePath);
        }
        public void Dispose()
        {
            if (TimerPay != null)
                TimerPay.Dispose();
            if (TimerLog != null)
                TimerLog.Dispose();
        }

        public void Start()
        {
            Interval = Settings.withdrawIntervalInMillis;
            TimerPay = new Timer(Interval);
            TimerPay.Elapsed += async (sender, e) => await Task.Run(() => RegularWithdraw(sender, e));
            TimerPay.Start();


            Interval = Settings.logIntervalInMillis;
            TimerLog = new Timer(Interval);
            TimerLog.Elapsed += async (sender, e) => await Task.Run(() => LogTransactions(sender, e));
            TimerLog.Start();

        }

        public void Stop()
        {
            if (TimerPay != null)
            {
                TimerPay.Stop();
            }
            if (TimerLog != null)
                TimerLog.Stop();
        }
        public void FireElapsedEvent()
        {
            Elapsed?.Invoke(this, null);
        }
        private void RegularWithdraw(object sender, ElapsedEventArgs e)
        {
            foreach (Vehicle v in parking.Vehicles)
            {
                decimal withdrawnSum = CalculateWithdrawnSum(v);
                v.Withdraw(withdrawnSum);
                parking.TransactionInfos.Add(ComposeTransactionInfo(v, withdrawnSum));
                Console.WriteLine("{1} was withdrawn from the vehicle with id {0}. Vehicle balance is {2}", v.Id, withdrawnSum, v.Balance);
            }
            Console.WriteLine("\n");
            TransactionInfo ComposeTransactionInfo(Vehicle vehicle, decimal withdrawnSum)
            {
                TransactionInfo transactionInfo = new TransactionInfo(vehicle.Balance, vehicle.Id, System.DateTime.Now);
                return transactionInfo;
            }

            decimal CalculateWithdrawnSum(Vehicle vehicle)
            {
                decimal withdrawnSum;
                Settings.vehicleParkingTariff.TryGetValue(vehicle.VehicleType, out withdrawnSum);
                if (vehicle.Balance >= withdrawnSum)
                {
                    return withdrawnSum;
                }
                else if (vehicle.Balance == 0)
                {
                    return withdrawnSum * Settings.penaltyMultiplier;
                }

                else if (vehicle.Balance < withdrawnSum && vehicle.Balance > 0)
                {
                    return ((withdrawnSum - vehicle.Balance) * Settings.penaltyMultiplier + (vehicle.Balance));
                }
                else if (vehicle.Balance < withdrawnSum)
                {
                    return withdrawnSum * Settings.penaltyMultiplier;
                }
                else
                {
                    return 0;
                }
            } 
        }
        private void LogTransactions(object sender, ElapsedEventArgs e)
        {
            var transaction = parking.TransactionInfos;
            for (int i = 0; i < transaction.Count; i++)
            {
                LogOneTransaction(transaction[i]);
            }
            parking.TransactionInfos.Clear();
            parking.CurentBalance = 0;

        }
        private void LogOneTransaction(TransactionInfo transaction) => LogService.Write($"{transaction.TimeTransaction:T}\n{transaction.VehicleId}\n{transaction.Sum}\n\n");
    }
}