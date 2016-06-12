using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoWeb.Areas.IP.Models
{
    public class WolManager
    {
        private static WolManager _instance = new WolManager();
        private Dictionary<string, WolTarget> _targetDict;
        public const double DEFAULT_VALID_INTERVAL = 0.5;
        private WolManager()
        {
            _targetDict = new Dictionary<string, WolTarget>();
        }
        public static WolManager Instance
        {
            get { return _instance; }
        }

        public bool IsTargetValid(string machineName)
        {
            if (_targetDict.ContainsKey(machineName))
            {
                if (_targetDict[machineName].IsValid())
                {
                    return true;

                }else
                {
                    _targetDict.Remove(machineName);
                    return false;
                }
            }
            return false;
        }
        public void RenewMachine(string machineName)
        {
            if (!_targetDict.ContainsKey(machineName) )
            {
                _targetDict[machineName] = new WolTarget(machineName);
            }
            _targetDict[machineName].Renew();
        }
    }
    public class WolTarget
    {
        public string MachineName { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan ValidInterval { get; set; }
        public WolTarget(string machineName)
        {
            MachineName = machineName;
            StartTime = DateTime.Now;
            ValidInterval = TimeSpan.FromMinutes(WolManager.DEFAULT_VALID_INTERVAL);
        }
        public bool IsValid()
        {
            if (DateTime.Now < StartTime + ValidInterval)
            {
                return true;
            }
            return false;
        }
        public void Renew()
        {
            StartTime = DateTime.Now;
            ValidInterval = TimeSpan.FromMinutes(WolManager.DEFAULT_VALID_INTERVAL);
        }
    }
}