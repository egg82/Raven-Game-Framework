using Raven.Display;
using Raven.Geom;
using Raven.Input.Enums;
using Raven.Input.Events;
using Raven.Utils;
using SharpDX.XInput;
using System;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public class Controllers : AbstractControllers {
        //vars
        public override event EventHandler<ButtonEventArgs> ButtonDown = null;
        public override event EventHandler<ButtonEventArgs> ButtonUp = null;
        public override event EventHandler<StickEventArgs> StickMoved = null;
        public override event EventHandler<TriggerEventArgs> TriggerPressed = null;

        private Controller[] controllers = new Controller[4];
        private Gamepad[] states = new Gamepad[4];
        private int[] packetNumbers = new int[4];
        private volatile int connectedControllers = 0;
        private readonly object updateLock = new object();

        private double stickDeadzone = 0.05d;
        private readonly object stickLock = new object();
        private double triggerDeadzone = 0.1d;
        private readonly object triggerLock = new object();
        private short dzPos = 0;
        private short dzNeg = 0;
        private byte dzTrig = 0;

        private readonly short maxAnglePos = 8192;
        private readonly short maxAngleNeg = -8193;

        //constructor
        internal Controllers(AtomicBoolean usingController) : base(usingController) {
            dzPos = (short) (stickDeadzone * 32767.0d);
            dzNeg = (short) (stickDeadzone * -32768.0d);
            dzTrig = (byte) (triggerDeadzone * 255.0d);

            controllers[0] = new Controller(UserIndex.One);
            controllers[1] = new Controller(UserIndex.Two);
            controllers[2] = new Controller(UserIndex.Three);
            controllers[3] = new Controller(UserIndex.Four);
            
            for (int i = 0; i < controllers.Length; i++) {
                if (controllers[i].IsConnected) {
                    connectedControllers++;
                    SharpDX.XInput.State state = controllers[i].GetState();
                    states[i] = state.Gamepad;
                    packetNumbers[i] = state.PacketNumber;
                } else {
                    states[i] = default(Gamepad);
                    packetNumbers[i] = int.MinValue;
                }
            }
        }

        //public
        public override double StickDeadzone {
            get {
                return stickDeadzone;
            }
            set {
                if (stickDeadzone == value || double.IsInfinity(value) || double.IsNaN(value)) {
                    return;
                }

                lock (stickLock) {
                    stickDeadzone = MathUtil.Clamp(0.0d, 1.0d, value);

                    dzPos = (short) (stickDeadzone * 32767.0d);
                    dzNeg = (short) (stickDeadzone * -32768.0d);
                }
            }
        }
        public override double TriggerDeadzone {
            get {
                return triggerDeadzone;
            }
            set {
                if (triggerDeadzone == value || double.IsInfinity(value) || double.IsNaN(value)) {
                    return;
                }

                lock (triggerLock) {
                    triggerDeadzone = MathUtil.Clamp(0.0d, 1.0d, value);

                    dzTrig = (byte) (triggerDeadzone * 255.0d);
                }
            }
        }
        public override int ConnectedControllers {
            get {
                return connectedControllers;
            }
        }

        public override bool IsAnyCodeFlagged(int controller, params int[] codes) {
            if (controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
                return false;
            }
            if (codes == null || codes.LongLength == 0L) {
                return true;
            }

            foreach (int b in codes) {
                if (b == (int) XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
                    return true;
                } else if (b == (int) XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
                    return true;
                } else {
                    if (b == (int) XboxStickCode.LeftN && states[controller].LeftThumbY >= dzPos && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftE && states[controller].LeftThumbX >= dzPos && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftS && states[controller].LeftThumbY <= dzNeg && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftW && states[controller].LeftThumbX <= dzNeg && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftNE && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX >= maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftNW && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX <= maxAngleNeg) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftSE && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX >= maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.LeftSW && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX <= maxAngleNeg) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightN && states[controller].RightThumbY >= dzPos && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightE && states[controller].RightThumbX >= dzPos && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightS && states[controller].RightThumbY <= dzNeg && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightW && states[controller].RightThumbX <= dzNeg && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightNE && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX >= maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightNW && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX <= maxAngleNeg) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightSE && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX >= maxAnglePos) {
                        return true;
                    } else if (b == (int) XboxStickCode.RightSW && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX <= maxAngleNeg) {
                        return true;
                    } else {
                        if (b == (int) XboxTriggerCode.Left && states[controller].LeftTrigger > dzTrig) {
                            return true;
                        } else if (b == (int) XboxTriggerCode.Right && states[controller].RightTrigger > dzTrig) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public override bool AreAllCodesFlagged(int controller, params int[] codes) {
            if (controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
                return false;
            }
            if (codes == null || codes.LongLength == 0L) {
                return true;
            }

            foreach (int b in codes) {
                if (b == (int) XboxButtonCode.A && (states[controller].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.B && (states[controller].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Y && (states[controller].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.X && (states[controller].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.LeftBumper && (states[controller].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.RightBumper && (states[controller].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.LeftStick && (states[controller].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.RightStick && (states[controller].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Start && (states[controller].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Back && (states[controller].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Up && (states[controller].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Down && (states[controller].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Left && (states[controller].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
                    continue;
                } else if (b == (int) XboxButtonCode.Right && (states[controller].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
                    continue;
                } else {
                    if (b == (int) XboxStickCode.LeftN && states[controller].LeftThumbY >= dzPos && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftE && states[controller].LeftThumbX >= dzPos && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftS && states[controller].LeftThumbY <= dzNeg && Math.Abs(states[controller].LeftThumbX) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftW && states[controller].LeftThumbX <= dzNeg && Math.Abs(states[controller].LeftThumbY) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftNE && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX >= maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftNW && states[controller].LeftThumbY >= dzPos && states[controller].LeftThumbX <= maxAngleNeg) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftSE && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX >= maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.LeftSW && states[controller].LeftThumbY <= dzNeg && states[controller].LeftThumbX <= maxAngleNeg) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightN && states[controller].RightThumbY >= dzPos && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightE && states[controller].RightThumbX >= dzPos && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightS && states[controller].RightThumbY <= dzNeg && Math.Abs(states[controller].RightThumbX) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightW && states[controller].RightThumbX <= dzNeg && Math.Abs(states[controller].RightThumbY) < maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightNE && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX >= maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightNW && states[controller].RightThumbY >= dzPos && states[controller].RightThumbX <= maxAngleNeg) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightSE && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX >= maxAnglePos) {
                        continue;
                    } else if (b == (int) XboxStickCode.RightSW && states[controller].RightThumbY <= dzNeg && states[controller].RightThumbX <= maxAngleNeg) {
                        continue;
                    } else {
                        if (b == (int) XboxTriggerCode.Left && states[controller].LeftTrigger > dzTrig) {
                            continue;
                        } else if (b == (int) XboxTriggerCode.Right && states[controller].RightTrigger > dzTrig) {
                            continue;
                        } else {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        public override double GetTriggerPressure(int controller, XboxSide side) {
            if (side == XboxSide.None || controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
                return 0.0d;
            }
            return (side == XboxSide.Left) ? states[controller].LeftTrigger / 255.0d : states[controller].RightTrigger / 255.0d;
        }
        public override PointD GetStickPosition(int controller, XboxSide side) {
            if (side == XboxSide.None || controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
                return new PointD();
            }
            return (side == XboxSide.Left) ? new PointD((states[controller].LeftThumbX < 0) ? states[controller].LeftThumbX / 32768.0d : states[controller].LeftThumbX / 32767.0d, (states[controller].LeftThumbY < 0) ? states[controller].LeftThumbY / 32768.0d : states[controller].LeftThumbY / 32767.0d) : new PointD((states[controller].RightThumbX < 0) ? states[controller].RightThumbX / 32768.0d : states[controller].RightThumbX / 32767.0d, (states[controller].RightThumbY < 0) ? states[controller].RightThumbY / 32768.0d : states[controller].RightThumbY / 32767.0d);
        }
        public override void Vibrate(int controller, double leftIntensity, double rightIntensity) {
            if (controller < 0 || controller >= controllers.Length || !controllers[controller].IsConnected) {
                return;
            }

            controllers[controller].SetVibration(new Vibration() {
                LeftMotorSpeed = (ushort) (MathUtil.Clamp(0.0d, 1.0d, leftIntensity) * 65535.0d),
                RightMotorSpeed = (ushort) (MathUtil.Clamp(0.0d, 1.0d, rightIntensity) * 65535.0d),
            });
        }

        //private
        internal override void Update(Window window) {
            if (!Supported) {
                usingController.Value = false;
                return;
            }

            lock (updateLock) {
                UpdateInternal(window);
            }
        }
        private void UpdateInternal(Window window) {
            int numControllers = 0;
            for (int i = 0; i < controllers.Length; i++) {
                if (!controllers[i].IsConnected) {
                    states[i] = default(Gamepad);
                    continue;
                }
                numControllers++;

                SharpDX.XInput.State state = controllers[i].GetState();
                if (state.PacketNumber == packetNumbers[i]) {
                    return;
                }
                packetNumbers[i] = state.PacketNumber;

                Gamepad pad = state.Gamepad;

                if ((pad.Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.A));
                } else if ((pad.Buttons & GamepadButtonFlags.A) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.A) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.A));
                }
                if ((pad.Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.B));
                } else if ((pad.Buttons & GamepadButtonFlags.B) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.B) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.B));
                }
                if ((pad.Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Y));
                } else if ((pad.Buttons & GamepadButtonFlags.Y) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Y) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Y));
                }
                if ((pad.Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.X));
                } else if ((pad.Buttons & GamepadButtonFlags.X) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.X) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.X));
                }

                if ((pad.Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.LeftBumper));
                } else if ((pad.Buttons & GamepadButtonFlags.LeftShoulder) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftShoulder) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.LeftBumper));
                }
                if ((pad.Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.RightBumper));
                } else if ((pad.Buttons & GamepadButtonFlags.RightShoulder) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightShoulder) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.RightBumper));
                }

                if ((pad.Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(this, new ButtonEventArgs(i, XboxButtonCode.LeftStick));
                } else if ((pad.Buttons & GamepadButtonFlags.LeftThumb) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.LeftThumb) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.LeftStick));
                }
                if ((pad.Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.RightStick));
                } else if ((pad.Buttons & GamepadButtonFlags.RightThumb) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.RightThumb) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.RightStick));
                }

                if ((pad.Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Start));
                } else if ((pad.Buttons & GamepadButtonFlags.Start) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Start) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Start));
                }
                if ((pad.Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Back));
                } else if ((pad.Buttons & GamepadButtonFlags.Back) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.Back) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Back));
                }

                if ((pad.Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Up));
                } else if ((pad.Buttons & GamepadButtonFlags.DPadUp) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadUp) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Up));
                }
                if ((pad.Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Down));
                } else if ((pad.Buttons & GamepadButtonFlags.DPadDown) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadDown) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Down));
                }
                if ((pad.Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Left));
                } else if ((pad.Buttons & GamepadButtonFlags.DPadLeft) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadLeft) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Left));
                }
                if ((pad.Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonDown?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Right));
                } else if ((pad.Buttons & GamepadButtonFlags.DPadRight) == GamepadButtonFlags.None && (states[i].Buttons & GamepadButtonFlags.DPadRight) != GamepadButtonFlags.None) {
                    usingController.Value = true;
                    ButtonUp?.Invoke(window, new ButtonEventArgs(i, XboxButtonCode.Right));
                }

                if (pad.LeftThumbY >= dzPos && pad.LeftThumbX < maxAnglePos && pad.LeftThumbX > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.North, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbX >= dzPos && pad.LeftThumbY < maxAnglePos && pad.LeftThumbY > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.East, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbY <= dzNeg && pad.LeftThumbX < maxAnglePos && pad.LeftThumbX > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.South, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbX <= dzNeg && pad.LeftThumbY < maxAnglePos && pad.LeftThumbY > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.West, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbY >= dzPos && pad.LeftThumbX >= maxAnglePos) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.NorthEast, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbY >= dzPos && pad.LeftThumbX <= maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.NorthWest, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbY <= dzNeg && pad.LeftThumbX >= maxAnglePos) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.SouthEast, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                } else if (pad.LeftThumbY <= dzNeg && pad.LeftThumbX <= maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Left, XboxStickDirection.SouthWest, (pad.LeftThumbX < 0) ? pad.LeftThumbX / 32768.0d : pad.LeftThumbX / 32767.0d, (pad.LeftThumbY < 0) ? pad.LeftThumbY / 32768.0d : pad.LeftThumbY / 32767.0d));
                }
                if (pad.RightThumbY >= dzPos && pad.RightThumbX < maxAnglePos && pad.RightThumbX > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.North, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbX >= dzPos && pad.RightThumbY < maxAnglePos && pad.RightThumbY > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.East, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbY <= dzNeg && pad.RightThumbX < maxAnglePos && pad.RightThumbX > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.South, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbX <= dzNeg && pad.RightThumbY < maxAnglePos && pad.RightThumbY > maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.West, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbY >= dzPos && pad.RightThumbX >= maxAnglePos) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.NorthEast, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbY >= dzPos && pad.RightThumbX <= maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.NorthWest, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbY <= dzNeg && pad.RightThumbX >= maxAnglePos) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.SouthEast, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                } else if (pad.RightThumbY <= dzNeg && pad.RightThumbX <= maxAngleNeg) {
                    usingController.Value = true;
                    StickMoved?.Invoke(window, new StickEventArgs(i, XboxSide.Right, XboxStickDirection.SouthWest, (pad.RightThumbX < 0) ? pad.RightThumbX / 32768.0d : pad.RightThumbX / 32767.0d, (pad.RightThumbY < 0) ? pad.RightThumbY / 32768.0d : pad.RightThumbY / 32767.0d));
                }

                if (pad.LeftTrigger >= dzTrig) {
                    usingController.Value = true;
                    TriggerPressed?.Invoke(window, new TriggerEventArgs(i, XboxSide.Left, pad.LeftTrigger / 255.0d));
                }
                if (pad.RightTrigger >= dzTrig) {
                    usingController.Value = true;
                    TriggerPressed?.Invoke(window, new TriggerEventArgs(i, XboxSide.Right, pad.RightTrigger / 255.0d));
                }

                states[i] = pad;
            }
            connectedControllers = numControllers;
        }
    }
}
