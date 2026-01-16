// Get, Setters in class
class MyComputer {
  private HasModel: string[] = [];

  public get GModel(): string[] {
    return this.HasModel;
  }

  public set SOneModel(model: string) {
    if (typeof model !== 'string' || !model)
      throw new Error(
        'Type Error: invalid input type in SOneModel() function. Allowed type: string',
      );

    this.HasModel = [...this.HasModel, model];
  }

  public set SModel(models: string[]) {
    if (!Array.isArray(this.HasModel) || !this.HasModel.every((e) => typeof e === 'string'))
      throw new Error('Type Error: Pass in value is not array string');

    this.HasModel = models;
  }
}

let modelList: string[] = ['Asus', 'MSI'];
let computers = new MyComputer();
computers.SModel = modelList; // set
console.log(computers.GModel); // get
computers.SOneModel = 'Acer';
console.log(computers.GModel);
