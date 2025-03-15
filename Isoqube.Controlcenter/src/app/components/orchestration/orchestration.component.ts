import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { OrchestrationService } from '../../services/orchestration.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Configuration } from '../../models/http';
import { SignalRService } from '../../services/signarR.service';

@Component({
  selector: 'app-orchestration',
  templateUrl: './orchestration.component.html',
  styleUrls: ['./orchestration.component.css'],
})
export class OrchestrationComponent implements OnInit {

  configurations$: Observable<any[]> | undefined;
  runs: any[] = [];
  bsModalRef!: BsModalRef;
  MD_MODAL_DIALOG_STYLE: ModalOptions = { class: 'modal-lg', backdrop: 'static' };
  @ViewChild('runConfiguration') runConfiguration!: TemplateRef<any>;

  context: { id?: string, description?: any, runConfiguration: Configuration } = { id: '', description: null, runConfiguration: { description: '', id: '', name: '', topics: [] } };

  constructor(private orchestrationService: OrchestrationService, private modalService: BsModalService, private signalRService: SignalRService) { }

  ngOnInit() {

    this.getAllRuns();
    this.getConfigurations();
    this.getTopicNotifications();   
  }

  getTopicNotifications() {
    this.signalRService.getMessages().subscribe((topicNotification) => {
      let currentRun = this.runs.find((run) => run.id === topicNotification.runId);
      let currentTopic = currentRun?.topics.find((topic: any) => topic.name === topicNotification.currentTopic.name);
      Object.assign(currentTopic, topicNotification.currentTopic);
    });
  }

  getConfigurations() {
    this.configurations$ = this.orchestrationService.getConfiguration();
  }

  getAllRuns() {
    this.orchestrationService.getAllruns().subscribe((runs) => {
      this.runs = runs;
    });
  }

  ngAfterViewInit() { }

  reRunConfiguration(run: any) {
    this.context = { description: run.description, runConfiguration: run };
    this.bsModalRef = this.modalService.show(this.runConfiguration, Object.assign({}, this.MD_MODAL_DIALOG_STYLE));
  }

  onCreateNewRun() {
    this.context = { description: '', runConfiguration: {} };
    this.bsModalRef = this.modalService.show(this.runConfiguration, Object.assign({}, this.MD_MODAL_DIALOG_STYLE));
  }

  onRunConfigurationc() {
    this.orchestrationService.runConfiguration(this.context).subscribe(() => {
      this.getAllRuns();
      this.bsModalRef.hide();
    });
  }

  closeModal() {
    this.bsModalRef.hide();
  }

  title = 'Orchestration Service';
}
