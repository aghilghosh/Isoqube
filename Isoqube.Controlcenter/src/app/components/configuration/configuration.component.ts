import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { OrchestrationService } from '../../services/orchestration.service';
import { Configuration, ITopic } from '../../models/http';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent {

  bsModalRef!: BsModalRef;
  @ViewChild('addConfiguration') addConfiguration!: TemplateRef<any>;
  configurations$: Observable<any[]> | undefined;
  MD_MODAL_DIALOG_STYLE: ModalOptions = { class: 'modal-lg', backdrop: 'static' };
  context: { selectedTopic: any, configuration: Configuration } = { selectedTopic: null, configuration: { description: '', id: '', name: '', topics: [] } };
  topics: ITopic[] = [];

  constructor(private orchestrationService: OrchestrationService, private modalService: BsModalService) {
  }

  ngOnInit() {
    this.getConfigurations();
    this.getRegisteredTopics();
  }

  title = 'configuration';

  private getRegisteredTopics() {
    this.orchestrationService.getAllTopics().subscribe(topics => this.topics = topics);
  }

  changeTopic() { }

  addNewConfiguration() {
    this.context.configuration = { description: '', id: '', name: '', topics: [] };
    this.bsModalRef = this.modalService.show(this.addConfiguration, Object.assign({}, this.MD_MODAL_DIALOG_STYLE));
  }

  modifyConfiguration(configuration: Configuration){
    this.context.configuration = configuration;
    this.bsModalRef = this.modalService.show(this.addConfiguration, Object.assign({}, this.MD_MODAL_DIALOG_STYLE));
  }

  onModifyConfiguration() {
    this.orchestrationService.addConfiguration(this.context.configuration).subscribe(() => {
      this.getConfigurations();
      this.bsModalRef.hide();
    });
  }

  getConfigurations() {
    this.configurations$ = this.orchestrationService.getConfiguration();
  }

  onTopicSelecred(topic: ITopic) {
    this.context.selectedTopic = topic;
  }

  onAddTopic() {
    if (this.context.selectedTopic && !this.context.configuration?.topics?.find(t => t.id === this.context.selectedTopic.id)) {
      this.context.configuration?.topics?.push(this.context.selectedTopic);
    }
  }

  removeTopic(topic: ITopic) {

    const index = this.context.configuration?.topics?.indexOf(topic);
    if (index !== undefined && index > -1) {
      this.context.configuration?.topics?.splice(index, 1);
    }
  }

  closeModal() {
    this.bsModalRef.hide();
  }
}
