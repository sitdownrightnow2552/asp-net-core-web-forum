<template>
  <crud-table
    :table="table"
    :form="form"
    title="Languages"
    url="languages"
    :filter="filter"
    form-width="400px"
  >
    <template #filter="{bind, on}">
      <v-text-field
        v-debounce="on.search"
        :value="bind.search"
        append-icon="search"
        label="Search"
        single-line
        hide-details
      >
      </v-text-field>
      <v-spacer class="d-none d-md-block"></v-spacer>
    </template>

    <template #form="{ values, inputs, errors }">
      <v-row>
        <v-col cols="12">
          <v-text-field
            :value="values.name"
            :error-messages="errors.name"
            @input="inputs.name"
            label="Name"
            persistent-placeholder
            required
          >
          </v-text-field>
        </v-col>
      </v-row>
    </template>

    <template #table.updatedAt="{ item }">
      {{ formatDate(item.updatedAt) }}
    </template>
  </crud-table>
</template>

<script lang="ts">
import { defineComponent, reactive } from '@vue/composition-api';
import { formatDate } from '@/composable/date';
import CrudTable from '@/components/CrudTable.vue';
import CrudEditForm from '@/components/CrudEditForm.vue';
import { usePriority } from '@/composable/form';

export default defineComponent({
  name: 'ManageLanguage',
  components: { CrudEditForm, CrudTable },
  setup() {
    const table = [
      {
        text: 'Id',
        sortable: false,
        value: 'slug',
      },
      { text: 'Name', value: 'name' },
      { text: 'Updated At', value: 'updatedAt' },
      { text: 'Action', value: 'action' },
    ];

    const filter = reactive({
      search: '',
    });

    const form = reactive({
      name: '',
    });

    return {
      table,
      filter,
      formatDate,
      form,
      getPriority: usePriority,
    };
  },
});
</script>
