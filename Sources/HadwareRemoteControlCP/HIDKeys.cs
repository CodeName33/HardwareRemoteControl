using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HadwareRemoteControlCP
{
    public static class HID
    {
        public enum HIDKeys
        {
            HID_KEY_NONE = 0x00,
            HID_KEY_A = 0x04,
            HID_KEY_B = 0x05,
            HID_KEY_C = 0x06,
            HID_KEY_D = 0x07,
            HID_KEY_E = 0x08,
            HID_KEY_F = 0x09,
            HID_KEY_G = 0x0A,
            HID_KEY_H = 0x0B,
            HID_KEY_I = 0x0C,
            HID_KEY_J = 0x0D,
            HID_KEY_K = 0x0E,
            HID_KEY_L = 0x0F,
            HID_KEY_M = 0x10,
            HID_KEY_N = 0x11,
            HID_KEY_O = 0x12,
            HID_KEY_P = 0x13,
            HID_KEY_Q = 0x14,
            HID_KEY_R = 0x15,
            HID_KEY_S = 0x16,
            HID_KEY_T = 0x17,
            HID_KEY_U = 0x18,
            HID_KEY_V = 0x19,
            HID_KEY_W = 0x1A,
            HID_KEY_X = 0x1B,
            HID_KEY_Y = 0x1C,
            HID_KEY_Z = 0x1D,
            HID_KEY_1 = 0x1E,
            HID_KEY_2 = 0x1F,
            HID_KEY_3 = 0x20,
            HID_KEY_4 = 0x21,
            HID_KEY_5 = 0x22,
            HID_KEY_6 = 0x23,
            HID_KEY_7 = 0x24,
            HID_KEY_8 = 0x25,
            HID_KEY_9 = 0x26,
            HID_KEY_0 = 0x27,
            HID_KEY_ENTER = 0x28,
            HID_KEY_ESCAPE = 0x29,
            HID_KEY_BACKSPACE = 0x2A,
            HID_KEY_TAB = 0x2B,
            HID_KEY_SPACE = 0x2C,
            HID_KEY_MINUS = 0x2D,
            HID_KEY_EQUAL = 0x2E,
            HID_KEY_BRACKET_LEFT = 0x2F,
            HID_KEY_BRACKET_RIGHT = 0x30,
            HID_KEY_BACKSLASH = 0x31,
            HID_KEY_EUROPE_1 = 0x32,
            HID_KEY_SEMICOLON = 0x33,
            HID_KEY_APOSTROPHE = 0x34,
            HID_KEY_GRAVE = 0x35,
            HID_KEY_COMMA = 0x36,
            HID_KEY_PERIOD = 0x37,
            HID_KEY_SLASH = 0x38,
            HID_KEY_CAPS_LOCK = 0x39,
            HID_KEY_F1 = 0x3A,
            HID_KEY_F2 = 0x3B,
            HID_KEY_F3 = 0x3C,
            HID_KEY_F4 = 0x3D,
            HID_KEY_F5 = 0x3E,
            HID_KEY_F6 = 0x3F,
            HID_KEY_F7 = 0x40,
            HID_KEY_F8 = 0x41,
            HID_KEY_F9 = 0x42,
            HID_KEY_F10 = 0x43,
            HID_KEY_F11 = 0x44,
            HID_KEY_F12 = 0x45,
            HID_KEY_PRINT_SCREEN = 0x46,
            HID_KEY_SCROLL_LOCK = 0x47,
            HID_KEY_PAUSE = 0x48,
            HID_KEY_INSERT = 0x49,
            HID_KEY_HOME = 0x4A,
            HID_KEY_PAGE_UP = 0x4B,
            HID_KEY_DELETE = 0x4C,
            HID_KEY_END = 0x4D,
            HID_KEY_PAGE_DOWN = 0x4E,
            HID_KEY_ARROW_RIGHT = 0x4F,
            HID_KEY_ARROW_LEFT = 0x50,
            HID_KEY_ARROW_DOWN = 0x51,
            HID_KEY_ARROW_UP = 0x52,
            HID_KEY_NUM_LOCK = 0x53,
            HID_KEY_KEYPAD_DIVIDE = 0x54,
            HID_KEY_KEYPAD_MULTIPLY = 0x55,
            HID_KEY_KEYPAD_SUBTRACT = 0x56,
            HID_KEY_KEYPAD_ADD = 0x57,
            HID_KEY_KEYPAD_ENTER = 0x58,
            HID_KEY_KEYPAD_1 = 0x59,
            HID_KEY_KEYPAD_2 = 0x5A,
            HID_KEY_KEYPAD_3 = 0x5B,
            HID_KEY_KEYPAD_4 = 0x5C,
            HID_KEY_KEYPAD_5 = 0x5D,
            HID_KEY_KEYPAD_6 = 0x5E,
            HID_KEY_KEYPAD_7 = 0x5F,
            HID_KEY_KEYPAD_8 = 0x60,
            HID_KEY_KEYPAD_9 = 0x61,
            HID_KEY_KEYPAD_0 = 0x62,
            HID_KEY_KEYPAD_DECIMAL = 0x63,
            HID_KEY_EUROPE_2 = 0x64,
            HID_KEY_APPLICATION = 0x65,
            HID_KEY_POWER = 0x66,
            HID_KEY_KEYPAD_EQUAL = 0x67,
            HID_KEY_F13 = 0x68,
            HID_KEY_F14 = 0x69,
            HID_KEY_F15 = 0x6A,
            HID_KEY_F16 = 0x6B,
            HID_KEY_F17 = 0x6C,
            HID_KEY_F18 = 0x6D,
            HID_KEY_F19 = 0x6E,
            HID_KEY_F20 = 0x6F,
            HID_KEY_F21 = 0x70,
            HID_KEY_F22 = 0x71,
            HID_KEY_F23 = 0x72,
            HID_KEY_F24 = 0x73,
            HID_KEY_EXECUTE = 0x74,
            HID_KEY_HELP = 0x75,
            HID_KEY_MENU = 0x76,
            HID_KEY_SELECT = 0x77,
            HID_KEY_STOP = 0x78,
            HID_KEY_AGAIN = 0x79,
            HID_KEY_UNDO = 0x7A,
            HID_KEY_CUT = 0x7B,
            HID_KEY_COPY = 0x7C,
            HID_KEY_PASTE = 0x7D,
            HID_KEY_FIND = 0x7E,
            HID_KEY_MUTE = 0x7F,
            HID_KEY_VOLUME_UP = 0x80,
            HID_KEY_VOLUME_DOWN = 0x81,
            HID_KEY_LOCKING_CAPS_LOCK = 0x82,
            HID_KEY_LOCKING_NUM_LOCK = 0x83,
            HID_KEY_LOCKING_SCROLL_LOCK = 0x84,
            HID_KEY_KEYPAD_COMMA = 0x85,
            HID_KEY_KEYPAD_EQUAL_SIGN = 0x86,
            HID_KEY_KANJI1 = 0x87,
            HID_KEY_KANJI2 = 0x88,
            HID_KEY_KANJI3 = 0x89,
            HID_KEY_KANJI4 = 0x8A,
            HID_KEY_KANJI5 = 0x8B,
            HID_KEY_KANJI6 = 0x8C,
            HID_KEY_KANJI7 = 0x8D,
            HID_KEY_KANJI8 = 0x8E,
            HID_KEY_KANJI9 = 0x8F,
            HID_KEY_LANG1 = 0x90,
            HID_KEY_LANG2 = 0x91,
            HID_KEY_LANG3 = 0x92,
            HID_KEY_LANG4 = 0x93,
            HID_KEY_LANG5 = 0x94,
            HID_KEY_LANG6 = 0x95,
            HID_KEY_LANG7 = 0x96,
            HID_KEY_LANG8 = 0x97,
            HID_KEY_LANG9 = 0x98,
            HID_KEY_ALTERNATE_ERASE = 0x99,
            HID_KEY_SYSREQ_ATTENTION = 0x9A,
            HID_KEY_CANCEL = 0x9B,
            HID_KEY_CLEAR = 0x9C,
            HID_KEY_PRIOR = 0x9D,
            HID_KEY_RETURN = 0x9E,
            HID_KEY_SEPARATOR = 0x9F,
            HID_KEY_OUT = 0xA0,
            HID_KEY_OPER = 0xA1,
            HID_KEY_CLEAR_AGAIN = 0xA2,
            HID_KEY_CRSEL_PROPS = 0xA3,
            HID_KEY_EXSEL = 0xA4,
            HID_KEY_KEYPAD_00 = 0xB0,
            HID_KEY_KEYPAD_000 = 0xB1,
            HID_KEY_THOUSANDS_SEPARATOR = 0xB2,
            HID_KEY_DECIMAL_SEPARATOR = 0xB3,
            HID_KEY_CURRENCY_UNIT = 0xB4,
            HID_KEY_CURRENCY_SUBUNIT = 0xB5,
            HID_KEY_KEYPAD_LEFT_PARENTHESIS = 0xB6,
            HID_KEY_KEYPAD_RIGHT_PARENTHESIS = 0xB7,
            HID_KEY_KEYPAD_LEFT_BRACE = 0xB8,
            HID_KEY_KEYPAD_RIGHT_BRACE = 0xB9,
            HID_KEY_KEYPAD_TAB = 0xBA,
            HID_KEY_KEYPAD_BACKSPACE = 0xBB,
            HID_KEY_KEYPAD_A = 0xBC,
            HID_KEY_KEYPAD_B = 0xBD,
            HID_KEY_KEYPAD_C = 0xBE,
            HID_KEY_KEYPAD_D = 0xBF,
            HID_KEY_KEYPAD_E = 0xC0,
            HID_KEY_KEYPAD_F = 0xC1,
            HID_KEY_KEYPAD_XOR = 0xC2,
            HID_KEY_KEYPAD_CARET = 0xC3,
            HID_KEY_KEYPAD_PERCENT = 0xC4,
            HID_KEY_KEYPAD_LESS_THAN = 0xC5,
            HID_KEY_KEYPAD_GREATER_THAN = 0xC6,
            HID_KEY_KEYPAD_AMPERSAND = 0xC7,
            HID_KEY_KEYPAD_DOUBLE_AMPERSAND = 0xC8,
            HID_KEY_KEYPAD_VERTICAL_BAR = 0xC9,
            HID_KEY_KEYPAD_DOUBLE_VERTICAL_BAR = 0xCA,
            HID_KEY_KEYPAD_COLON = 0xCB,
            HID_KEY_KEYPAD_HASH = 0xCC,
            HID_KEY_KEYPAD_SPACE = 0xCD,
            HID_KEY_KEYPAD_AT = 0xCE,
            HID_KEY_KEYPAD_EXCLAMATION = 0xCF,
            HID_KEY_KEYPAD_MEMORY_STORE = 0xD0,
            HID_KEY_KEYPAD_MEMORY_RECALL = 0xD1,
            HID_KEY_KEYPAD_MEMORY_CLEAR = 0xD2,
            HID_KEY_KEYPAD_MEMORY_ADD = 0xD3,
            HID_KEY_KEYPAD_MEMORY_SUBTRACT = 0xD4,
            HID_KEY_KEYPAD_MEMORY_MULTIPLY = 0xD5,
            HID_KEY_KEYPAD_MEMORY_DIVIDE = 0xD6,
            HID_KEY_KEYPAD_PLUS_MINUS = 0xD7,
            HID_KEY_KEYPAD_CLEAR = 0xD8,
            HID_KEY_KEYPAD_CLEAR_ENTRY = 0xD9,
            HID_KEY_KEYPAD_BINARY = 0xDA,
            HID_KEY_KEYPAD_OCTAL = 0xDB,
            HID_KEY_KEYPAD_DECIMAL_2 = 0xDC,
            HID_KEY_KEYPAD_HEXADECIMAL = 0xDD,
            HID_KEY_CONTROL_LEFT = 0xE0,
            HID_KEY_SHIFT_LEFT = 0xE1,
            HID_KEY_ALT_LEFT = 0xE2,
            HID_KEY_GUI_LEFT = 0xE3,
            HID_KEY_CONTROL_RIGHT = 0xE4,
            HID_KEY_SHIFT_RIGHT = 0xE5,
            HID_KEY_ALT_RIGHT = 0xE6,
            HID_KEY_GUI_RIGHT = 0xE7,
        }

        static Dictionary<int, List<HIDKeys>> KeysMap = null;

        static void InitMapInt()
        {
            KeysMap = new Dictionary<int, List<HIDKeys>> {
                { 33, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_1 } },
                { 64, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_2 } },
                { 35, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_3 } },
                { 36, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_4 } },
                { 37, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_5 } },
                { 94, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_6 } },
                { 38, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_7 } },
                { 42, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_8 } },
                { 40, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_9 } },
                { 41, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_0 } },
                { 96, new List<HIDKeys> { HIDKeys.HID_KEY_GRAVE } },
                { 126, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_GRAVE } },
                { 45, new List<HIDKeys> { HIDKeys.HID_KEY_MINUS } },
                { 95, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_MINUS } },
                { 61, new List<HIDKeys> { HIDKeys.HID_KEY_EQUAL } },
                { 43, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_EQUAL } },
                { 44, new List<HIDKeys> { HIDKeys.HID_KEY_COMMA} },
                { 60, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_COMMA } },
                { 46, new List<HIDKeys> { HIDKeys.HID_KEY_PERIOD} },
                { 62, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_PERIOD } },
                { 91, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_LEFT} },
                { 123, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BRACKET_LEFT } },
                { 93, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_RIGHT} },
                { 125, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BRACKET_RIGHT } },
                { 47, new List<HIDKeys> { HIDKeys.HID_KEY_SLASH} },
                { 63, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_SLASH } },
                { 92, new List<HIDKeys> { HIDKeys.HID_KEY_BACKSLASH} },
                { 124, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BACKSLASH } },
                { 59, new List<HIDKeys> { HIDKeys.HID_KEY_SEMICOLON} },
                { 58, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_SEMICOLON } },
                { 39, new List<HIDKeys> { HIDKeys.HID_KEY_APOSTROPHE} },
                { 34, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_APOSTROPHE } },
                { 13, new List<HIDKeys> { HIDKeys.HID_KEY_ENTER } },
                { 27, new List<HIDKeys> { HIDKeys.HID_KEY_ESCAPE } },
                { 32, new List<HIDKeys> { HIDKeys.HID_KEY_SPACE } },
                { 9, new List<HIDKeys> { HIDKeys.HID_KEY_TAB } },


                { 2490368, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_UP } },
                { 2621440, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_DOWN } },
                { 2424832, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_LEFT } },
                { 2555904, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_RIGHT } },

                { 2949120, new List<HIDKeys> { HIDKeys.HID_KEY_INSERT } },
                { 3014656, new List<HIDKeys> { HIDKeys.HID_KEY_DELETE } },
                { 2359296, new List<HIDKeys> { HIDKeys.HID_KEY_HOME} },
                { 2293760, new List<HIDKeys> { HIDKeys.HID_KEY_END } },
                { 2162688, new List<HIDKeys> { HIDKeys.HID_KEY_PAGE_UP } },
                { 2228224, new List<HIDKeys> { HIDKeys.HID_KEY_PAGE_DOWN } },

                { 7340032, new List<HIDKeys> { HIDKeys.HID_KEY_F1 } },
                { 7405568, new List<HIDKeys> { HIDKeys.HID_KEY_F2 } },
                { 7471104, new List<HIDKeys> { HIDKeys.HID_KEY_F3 } },
                { 7536640, new List<HIDKeys> { HIDKeys.HID_KEY_F4 } },
                { 7602176, new List<HIDKeys> { HIDKeys.HID_KEY_F5 } },
                { 7667712, new List<HIDKeys> { HIDKeys.HID_KEY_F6 } },
                { 7733248, new List<HIDKeys> { HIDKeys.HID_KEY_F7 } },
                { 7798784, new List<HIDKeys> { HIDKeys.HID_KEY_F8 } },
                { 7864320, new List<HIDKeys> { HIDKeys.HID_KEY_F9 } },
                { 7929856, new List<HIDKeys> { HIDKeys.HID_KEY_F10 } },
                { 7995392, new List<HIDKeys> { HIDKeys.HID_KEY_F11 } },
                { 8060928, new List<HIDKeys> { HIDKeys.HID_KEY_F12 } },
            };

            for (int i = 97; i <= 122; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_A + (i - 97) };
            }
            for (int i = 65; i <= 90; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_A + (i - 65) };
            }
            for (int i = 1; i <= 26; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_CONTROL_LEFT, HIDKeys.HID_KEY_A + (i - 1) };
            }
            for (int i = 48; i <= 57; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_0 + (i - 48) };
            }
        }

        static void InitMap()
        {
            KeysMap = new Dictionary<int, List<HIDKeys>> {
                /*
                { 33, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_1 } },
                { 64, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_2 } },
                { 35, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_3 } },
                { 36, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_4 } },
                { 37, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_5 } },
                { 94, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_6 } },
                { 38, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_7 } },
                { 42, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_8 } },
                { 40, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_9 } },
                { 41, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_0 } },*/
                { 33, new List<HIDKeys> { HIDKeys.HID_KEY_1 } },
                { 64, new List<HIDKeys> { HIDKeys.HID_KEY_2 } },
                { 35, new List<HIDKeys> { HIDKeys.HID_KEY_3 } },
                { 36, new List<HIDKeys> { HIDKeys.HID_KEY_4 } },
                { 37, new List<HIDKeys> { HIDKeys.HID_KEY_5 } },
                { 94, new List<HIDKeys> { HIDKeys.HID_KEY_6 } },
                { 38, new List<HIDKeys> { HIDKeys.HID_KEY_7 } },
                { 42, new List<HIDKeys> { HIDKeys.HID_KEY_8 } },
                { 40, new List<HIDKeys> { HIDKeys.HID_KEY_9 } },
                { 41, new List<HIDKeys> { HIDKeys.HID_KEY_0 } },

                { 96, new List<HIDKeys> { HIDKeys.HID_KEY_GRAVE } },
                //{ 126, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_GRAVE } },
                { 126, new List<HIDKeys> { HIDKeys.HID_KEY_GRAVE } },
                { 45, new List<HIDKeys> { HIDKeys.HID_KEY_MINUS } },
                //{ 95, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_MINUS } },
                { 95, new List<HIDKeys> { HIDKeys.HID_KEY_MINUS } },
                { 61, new List<HIDKeys> { HIDKeys.HID_KEY_EQUAL } },
                //{ 43, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_EQUAL } },
                { 43, new List<HIDKeys> { HIDKeys.HID_KEY_EQUAL } },
                { 44, new List<HIDKeys> { HIDKeys.HID_KEY_COMMA} },
                //{ 60, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_COMMA } },
                { 60, new List<HIDKeys> { HIDKeys.HID_KEY_COMMA } },
                { 46, new List<HIDKeys> { HIDKeys.HID_KEY_PERIOD} },
                //{ 62, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_PERIOD } },
                { 62, new List<HIDKeys> { HIDKeys.HID_KEY_PERIOD } },
                { 91, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_LEFT} },
                //{ 123, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BRACKET_LEFT } },
                { 123, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_LEFT } },
                { 93, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_RIGHT} },
                //{ 125, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BRACKET_RIGHT } },
                { 125, new List<HIDKeys> { HIDKeys.HID_KEY_BRACKET_RIGHT } },
                { 47, new List<HIDKeys> { HIDKeys.HID_KEY_SLASH} },
                //{ 63, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_SLASH } },
                { 63, new List<HIDKeys> { HIDKeys.HID_KEY_SLASH } },
                { 92, new List<HIDKeys> { HIDKeys.HID_KEY_BACKSLASH} },
                //{ 124, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_BACKSLASH } },
                { 124, new List<HIDKeys> { HIDKeys.HID_KEY_BACKSLASH } },
                { 59, new List<HIDKeys> { HIDKeys.HID_KEY_SEMICOLON} },
                //{ 58, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_SEMICOLON } },
                { 58, new List<HIDKeys> { HIDKeys.HID_KEY_SEMICOLON } },
                { 39, new List<HIDKeys> { HIDKeys.HID_KEY_APOSTROPHE} },
                //{ 34, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_APOSTROPHE } },
                { 34, new List<HIDKeys> { HIDKeys.HID_KEY_APOSTROPHE } },
                { 65293, new List<HIDKeys> { HIDKeys.HID_KEY_ENTER } },
                { 65307, new List<HIDKeys> { HIDKeys.HID_KEY_ESCAPE } },
                { 65505, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT } },
                { 65506, new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_RIGHT } },
                { 65507, new List<HIDKeys> { HIDKeys.HID_KEY_CONTROL_LEFT } },
                { 65508, new List<HIDKeys> { HIDKeys.HID_KEY_CONTROL_RIGHT } },
                { 65513, new List<HIDKeys> { HIDKeys.HID_KEY_ALT_LEFT } },
                { 65514, new List<HIDKeys> { HIDKeys.HID_KEY_ALT_RIGHT } },
                { 65511, new List<HIDKeys> { HIDKeys.HID_KEY_GUI_LEFT} },
                { 65300, new List<HIDKeys> { HIDKeys.HID_KEY_SCROLL_LOCK} },
                { 65407, new List<HIDKeys> { HIDKeys.HID_KEY_PAUSE} },
                { 32, new List<HIDKeys> { HIDKeys.HID_KEY_SPACE } },
                { 65289, new List<HIDKeys> { HIDKeys.HID_KEY_TAB } },
                { 65288, new List<HIDKeys> { HIDKeys.HID_KEY_BACKSPACE } },



                { 65362, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_UP } },
                { 65364, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_DOWN } },
                { 65361, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_LEFT } },
                { 65363, new List<HIDKeys> { HIDKeys.HID_KEY_ARROW_RIGHT } },

                { 65379, new List<HIDKeys> { HIDKeys.HID_KEY_INSERT } },
                { 65535, new List<HIDKeys> { HIDKeys.HID_KEY_DELETE } },
                { 65360, new List<HIDKeys> { HIDKeys.HID_KEY_HOME} },
                { 65367, new List<HIDKeys> { HIDKeys.HID_KEY_END } },
                { 65365, new List<HIDKeys> { HIDKeys.HID_KEY_PAGE_UP } },
                { 65366, new List<HIDKeys> { HIDKeys.HID_KEY_PAGE_DOWN } },

                { 65470, new List<HIDKeys> { HIDKeys.HID_KEY_F1 } },
                { 65471, new List<HIDKeys> { HIDKeys.HID_KEY_F2 } },
                { 65472, new List<HIDKeys> { HIDKeys.HID_KEY_F3 } },
                { 65473, new List<HIDKeys> { HIDKeys.HID_KEY_F4 } },
                { 65474, new List<HIDKeys> { HIDKeys.HID_KEY_F5 } },
                { 65475, new List<HIDKeys> { HIDKeys.HID_KEY_F6 } },
                { 65476, new List<HIDKeys> { HIDKeys.HID_KEY_F7 } },
                { 65477, new List<HIDKeys> { HIDKeys.HID_KEY_F8 } },
                { 65478, new List<HIDKeys> { HIDKeys.HID_KEY_F9 } },
                { 65479, new List<HIDKeys> { HIDKeys.HID_KEY_F10 } },
                { 65480, new List<HIDKeys> { HIDKeys.HID_KEY_F11 } },
                { 65481, new List<HIDKeys> { HIDKeys.HID_KEY_F12 } },
            };

            for (int i = 97; i <= 122; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_A + (i - 97) };
            }
            for (int i = 65; i <= 90; i++)
            {
                //KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_SHIFT_LEFT, HIDKeys.HID_KEY_A + (i - 65) };
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_A + (i - 65) };
            }
            /*
            for (int i = 1; i <= 26; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_CONTROL_LEFT, HIDKeys.HID_KEY_A + (i - 1) };
            }*/
            for (int i = 48; i <= 57; i++)
            {
                KeysMap[i] = new List<HIDKeys> { HIDKeys.HID_KEY_0 + (i - 48) };
            }
        }

        public static List<HIDKeys> GetHIDKeys(int Key)
        {
            if (KeysMap == null)
            {
                InitMap();
            }
            if (KeysMap != null && KeysMap.TryGetValue(Key, out var list))
            {
                return list;
            }
            return new List<HIDKeys>();
        }
    }
}
