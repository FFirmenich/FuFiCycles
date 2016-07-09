using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.FuFiCycles.Core {
	public enum Direction {
		FORWARD,
		RIGHT,
		BACKWARD,
		LEFT
	}
	static class DirectionMethods {
		public static float getYaw(this Direction direction) {
			switch (direction) {
				case Direction.RIGHT:
					return M.PiOver2;
				case Direction.FORWARD:
					return 0;
				case Direction.LEFT:
					return -M.PiOver2;
				case Direction.BACKWARD:
					return M.Pi;
				default:
					throw (new Exception("no yaw for direction found"));
			}
		}

		public static Direction directionFromYaw(float yaw) {
			float val = 0.1f;
			if (yaw < M.PiOver2 + val && yaw > M.PiOver2 - val) {
				return Direction.RIGHT;
			} else if (yaw > -val && yaw < val || yaw < M.TwoPi + val && yaw > M.TwoPi - val) {
				return Direction.FORWARD;
			} else if (yaw < -M.PiOver2 + val && yaw > -M.PiOver2 - val || yaw < M.ThreePiOver2 + val && yaw > M.ThreePiOver2 - val) {
				return Direction.LEFT;
			} else if (yaw > M.Pi - val && yaw < M.Pi + val || yaw > -M.Pi - val && yaw < -M.Pi + val) {
				return Direction.BACKWARD;
			}
			throw (new Exception("no direction for yaw found"));
		}
	}
}
