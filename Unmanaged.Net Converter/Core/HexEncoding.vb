﻿Imports System

Namespace Core.Encodings
    Public Class HexEncoding

        Private Shared ReadOnly _byteToCharPairLookup As UShort() = {&H3030, &H3130, &H3230, &H3330, &H3430, &H3530, &H3630, &H3730, &H3830, &H3930, &H4130, &H4230, &H4330, &H4430, &H4530, &H4630, &H3031, &H3131, &H3231, &H3331, &H3431, &H3531, &H3631, &H3731, &H3831, &H3931, &H4131, &H4231, &H4331, &H4431, &H4531, &H4631, &H3032, &H3132, &H3232, &H3332, &H3432, &H3532, &H3632, &H3732, &H3832, &H3932, &H4132, &H4232, &H4332, &H4432, &H4532, &H4632, &H3033, &H3133, &H3233, &H3333, &H3433, &H3533, &H3633, &H3733, &H3833, &H3933, &H4133, &H4233, &H4333, &H4433, &H4533, &H4633, &H3034, &H3134, &H3234, &H3334, &H3434, &H3534, &H3634, &H3734, &H3834, &H3934, &H4134, &H4234, &H4334, &H4434, &H4534, &H4634, &H3035, &H3135, &H3235, &H3335, &H3435, &H3535, &H3635, &H3735, &H3835, &H3935, &H4135, &H4235, &H4335, &H4435, &H4535, &H4635, &H3036, &H3136, &H3236, &H3336, &H3436, &H3536, &H3636, &H3736, &H3836, &H3936, &H4136, &H4236, &H4336, &H4436, &H4536, &H4636, &H3037, &H3137, &H3237, &H3337, &H3437, &H3537, &H3637, &H3737, &H3837, &H3937, &H4137, &H4237, &H4337, &H4437, &H4537, &H4637, &H3038, &H3138, &H3238, &H3338, &H3438, &H3538, &H3638, &H3738, &H3838, &H3938, &H4138, &H4238, &H4338, &H4438, &H4538, &H4638, &H3039, &H3139, &H3239, &H3339, &H3439, &H3539, &H3639, &H3739, &H3839, &H3939, &H4139, &H4239, &H4339, &H4439, &H4539, &H4639, &H3041, &H3141, &H3241, &H3341, &H3441, &H3541, &H3641, &H3741, &H3841, &H3941, &H4141, &H4241, &H4341, &H4441, &H4541, &H4641, &H3042, &H3142, &H3242, &H3342, &H3442, &H3542, &H3642, &H3742, &H3842, &H3942, &H4142, &H4242, &H4342, &H4442, &H4542, &H4642, &H3043, &H3143, &H3243, &H3343, &H3443, &H3543, &H3643, &H3743, &H3843, &H3943, &H4143, &H4243, &H4343, &H4443, &H4543, &H4643, &H3044, &H3144, &H3244, &H3344, &H3444, &H3544, &H3644, &H3744, &H3844, &H3944, &H4144, &H4244, &H4344, &H4444, &H4544, &H4644, &H3045, &H3145, &H3245, &H3345, &H3445, &H3545, &H3645, &H3745, &H3845, &H3945, &H4145, &H4245, &H4345, &H4445, &H4545, &H4645, &H3046, &H3146, &H3246, &H3346, &H3446, &H3546, &H3646, &H3746, &H3846, &H3946, &H4146, &H4246, &H4346, &H4446, &H4546, &H4646}
        Private Shared ReadOnly _charToNibbleLookup As UShort() = {&HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HA, &HB, &HC, &HD, &HE, &HF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HA, &HB, &HC, &HD, &HE, &HF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF, &HFFFF}

        Public Shared Function GetString(ByVal sourceBytes As Byte()) As List(Of String)
            If sourceBytes Is Nothing Then Throw New ArgumentNullException(NameOf(sourceBytes))
            Dim outputChars = New Char(sourceBytes.Length * 2 - 1) {}
            Dim Result As New List(Of String)

            For i = 0 To sourceBytes.Length - 1
                Dim characterPair = _byteToCharPairLookup(sourceBytes(i))
                outputChars(2 * i) = Microsoft.VisualBasic.ChrW(characterPair And &HFF)
                outputChars(2 * i + 1) = Microsoft.VisualBasic.ChrW(characterPair >> 8)
                Dim HexPart As String = outputChars(2 * i).ToString & outputChars(2 * i + 1).ToString
                Result.Add(HexPart)
            Next

            '  Dim HexResult As String = New String(outputChars)

            Return Result '.Substring(0, Result.Count - 2).ToString
        End Function

    End Class
End Namespace
