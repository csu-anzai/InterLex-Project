import {Component, OnInit} from '@angular/core';
import {HttpService} from '../../../core/services/http.service';
import {SuggestionsTreeModel} from '../../../models/suggestions-tree.model';
import {TreeNode} from 'primeng/api';
import {countryList} from '../../../models/case-editor.model';

@Component({
  selector: 'app-suggestions',
  templateUrl: './suggestions.component.html',
  styleUrls: ['./suggestions.component.css']
})
export class SuggestionsComponent implements OnInit {

  dbModel: SuggestionsTreeModel[];
  nodes: TreeNode[] = [];


  constructor(private http: HttpService) {
  }

  ngOnInit() {
    this.http.get('Case/GetAllSuggestions').subscribe((x: SuggestionsTreeModel[]) => {
      this.dbModel = x;
      this.fillTreeModel();
    });
  }

  fillTreeModel() {
    const arr: TreeNode[] = [];
    const indexOfKeywordItem = this.dbModel.findIndex(x => x.jurisdictionCode.toLowerCase() === 'keywords');
    if (indexOfKeywordItem >= 0) {
      const node = {data: {label: 'Keywords'}, children: []};
      const keywords = this.dbModel.splice(indexOfKeywordItem, 1)[0];
      for (const keyword of keywords.keywords) {
        node.children.push({data: keyword, leaf: true});
      }
      arr.push(node);
    }

    for (const jur of this.dbModel) {
      const jurFullName = countryList.filter(x => x.code === jur.jurisdictionCode)[0];
      const node = {data: jurFullName, children: []};
      if (jur.courts.length) {
        const court = {data: 'Court', children: []};
        node.children.push(court);
        for (const value of jur.courts) {
          const leaf = {data: value, leaf: true};
          court.children.push(leaf);
        }
      }
      if (jur.courtsEng.length) {
        const court = {data: 'CourtEng', children: []};
        node.children.push(court);
        for (const value of jur.courtsEng) {
          const leaf = {data: value, leaf: true};
          court.children.push(leaf);
        }
      }
      if (jur.sources.length) {
        const source = {data: 'Source', children: []};
        node.children.push(source);
        for (const value of jur.sources) {
          const leaf = {data: value, leaf: true};
          source.children.push(leaf);
        }
      }
      arr.push(node);
    }
    this.nodes = arr;
  }

  deleteSuggestion(item: { node: TreeNode, parent: TreeNode }) {
    const name = item.node.data; // the suggestion text itself
    let type = item.parent.data; // the field to which the suggestion is bound e.g. court, source...
    let jurCode;
    if (typeof type === 'string') {
      jurCode = item.parent.parent.data.code; // the code of the jurisdiction
    } else {
      type = item.parent.data.label;
    }
    this.http.post('Case/DeleteSuggestion', {name, type, jurCode})
      .subscribe(x => {
        item.parent.children = item.parent.children.filter(node => node !== item.node);
        this.nodes = [...this.nodes];
      }, err => {
        console.log(err);
      });
  }
}
