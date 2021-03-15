using System;

[Serializable]
class UpdateSharkNameMessage : Message
{
    public uint sharkId;
    public String name;

    public UpdateSharkNameMessage(Messages type, uint sharkId, String name) : base(type)
    {
        this.sharkId = sharkId;
        this.name = name;
    }
}
