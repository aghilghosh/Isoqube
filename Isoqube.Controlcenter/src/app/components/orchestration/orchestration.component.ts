import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { OrchestrationService } from '../../services/orchestration.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Configuration } from '../../models/http';

@Component({
  selector: 'app-orchestration',
  templateUrl: './orchestration.component.html',
  styleUrls: ['./orchestration.component.css'],
})
export class OrchestrationComponent implements OnInit {

  configurations$: Observable<any[]> | undefined;
  runs$: Observable<any[]> | undefined;
  bsModalRef!: BsModalRef;
  MD_MODAL_DIALOG_STYLE: ModalOptions = { class: 'modal-lg', backdrop: 'static' };
  @ViewChild('runConfiguration') runConfiguration!: TemplateRef<any>;

  context: { id?: string, description?: any, runConfiguration: Configuration } = { id: '', description: null, runConfiguration: { description: '', id: '', name: '', topics: [] } };

  constructor(private orchestrationService: OrchestrationService, private modalService: BsModalService) { }

  ngOnInit() {
    this.getAllRuns();
    this.getConfigurations();
  }

  getConfigurations() {
    this.configurations$ = this.orchestrationService.getConfiguration();
  }

  getAllRuns() {
    this.runs$ = this.orchestrationService.getAllruns();
  }

  ngAfterViewInit() { }

  reRunConfiguration(run: any) {
    this.context = { id: run.id, runConfiguration: run };
    this.orchestrationService.runConfiguration(this.context).subscribe(() => {
      this.getAllRuns();
      this.bsModalRef.hide();
    });
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
