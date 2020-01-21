//using ForgeModGenerator.Serialization;
//using ForgeModGenerator.SoundGenerator.Models;
//using ForgeModGenerator.Validation;
//using System.Collections.Generic;

//namespace ForgeModGenerator.SoundGenerator.Serialization
//{
//    public class SoundJsonUpdater : CollectionJsonUpdater<IEnumerable<SoundEvent>, SoundEvent>
//    {
//        public SoundJsonUpdater(ISerializer<IEnumerable<SoundEvent>, SoundEvent> serializer, IEnumerable<SoundEvent> target, string jsonPath) : base(serializer, target, jsonPath) { }
        
//        public override bool IsValidToSerialize()
//        {
//            foreach (SoundEvent soundEvent in Target)
//            {
//                ValidateResult result = soundEvent.Validate();
//                if (!result.IsValid)
//                {
//                    Log.Warning($"Cannot serialize json. {soundEvent.EventName} is not valid. Reason: {result.Error}", true);
//                    return false;
//                }
//            }
//            return true;
//        }
//    }
//}
